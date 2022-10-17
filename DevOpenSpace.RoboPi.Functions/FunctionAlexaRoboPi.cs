using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Azure.Messaging.ServiceBus;
using DevOpenSpace.RoboPi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.LocaleSpeech;

namespace DevOpenSpace.RoboPi.Functions
{
    public class FunctionAlexaRoboPi
    {
        private readonly ILogger<FunctionAlexaRoboPi> _logger;
        private IIntentService _intentService;

        public FunctionAlexaRoboPi(ILogger<FunctionAlexaRoboPi> log)
        {
            _logger = log;
            _intentService = new IntentService();
        }

        [FunctionName("AlexaRoboPi")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string json = await req.ReadAsStringAsync();
            _logger.LogInformation(json);

            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            SkillResponse response = null;

            if (skillRequest == null)
            {
                response = ResponseBuilder.Tell("SkillRequest konnte nicht gelesen werden.");
                response.Response.ShouldEndSession = false;
                
                return new OkObjectResult(response);
            }

            var locale = SetupLanguages(skillRequest);

            bool isValid = await ValidateRequest(req, _logger, skillRequest);
            if (!isValid)
            {
                return new BadRequestResult();
            }

            var requestType = skillRequest.GetRequestType();
            

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Willkommen beim DevOpenSpace!");
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                var intentName = intentRequest?.Intent.Name;
                _logger.LogInformation(intentName);
                if (!string.IsNullOrEmpty(intentName))
                {
                    var intentMessages = _intentService.GetMessages(intentName);
                    intentMessages.ForEach(intentMessage =>
                    {
                        _logger.LogInformation(intentMessage.Message);
                    });

                    if (intentRequest.Intent.Name == "AMAZON.CancelIntent")
                    {
                        var message = await locale.Get("Cancel", null);
                        return new OkObjectResult(ResponseBuilder.Tell(message));
                    }
                    else if (intentRequest.Intent.Name == "AMAZON.HelpIntent")
                    {
                        var message = await locale.Get("Help", null);
                        response = ResponseBuilder.Tell(message);
                        response.Response.ShouldEndSession = false;
                        return new OkObjectResult(response);
                    }
                    else if (intentRequest.Intent.Name == "AMAZON.StopIntent")
                    {
                        var message = await locale.Get("Stop", null);
                        return new OkObjectResult(ResponseBuilder.Tell(message));
                    }

                    string connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                    ServiceBusClient client = new ServiceBusClient(connectionString);

                    foreach (var intentMessage in intentMessages)
                    {
                        var serviceBusMessage = new ServiceBusMessage(intentMessage.Intent.ToString());

                        if (!string.IsNullOrEmpty(connectionString) && intentMessage.RoboPiNumber > 0)
                        {
                            var sender = client.CreateSender($"robopicarnumber{intentMessage.RoboPiNumber}");
                            await sender.SendMessageAsync(serviceBusMessage, new CancellationToken());
                        }
                    }

                    response = ResponseBuilder.Tell(string.Join(" ", intentMessages.Select(i => i.Message)));
                    response.Response.ShouldEndSession = false;
                }
            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                _logger.LogInformation("Session ended");
                response = ResponseBuilder.Empty();
                response.Response.ShouldEndSession = true;
            }

            return new OkObjectResult(response);
        }

        private static async Task<bool> ValidateRequest(HttpRequest request, ILogger log, SkillRequest skillRequest)
        {
            request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureChainUrl);
            if (string.IsNullOrWhiteSpace(signatureChainUrl))
            {
                log.LogError("Validation failed. Empty SignatureCertChainUrl header");
                return false;
            }

            Uri certUrl;
            try
            {
                certUrl = new Uri(signatureChainUrl);
            }
            catch
            {
                log.LogError($"Validation failed. SignatureChainUrl not valid: {signatureChainUrl}");
                return false;
            }

            request.Headers.TryGetValue("Signature", out var signature);
            if (string.IsNullOrWhiteSpace(signature))
            {
                log.LogError("Validation failed - Empty Signature header");
                return false;
            }

            request.Body.Position = 0;
            var body = await request.ReadAsStringAsync();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                log.LogError("Validation failed - the JSON is empty");
                return false;
            }

            bool isTimestampValid = RequestVerification.RequestTimestampWithinTolerance(skillRequest);
            bool valid = await RequestVerification.Verify(signature, certUrl, body);

            if (!valid || !isTimestampValid)
            {
                log.LogError("Validation failed - RequestVerification failed");
                return false;
            }
            else
            {
                return true;
            }
        }

        private static ILocaleSpeech SetupLanguages(SkillRequest skillRequest)
        {
            var store = new DictionaryLocaleSpeechStore();
            store.AddLanguage("en", new Dictionary<string, object>
            {
                { "Welcome", "Welcome to DevOpenSpace!" },
                { "StartEins", "Start Robo Pi 1." },
                { "Cancel", "I'm cancelling the request..." },
                { "Help", "You can ask me, for example, to turn Robo Pi One left." },
                { "Stop", "Goodbye!" }
            });

            store.AddLanguage("de", new Dictionary<string, object>
            {
                { "Welcome", "Willkommen beim DevOpenSpace!" },
                { "StartEins", "Starte Robo Pi 1." },
                { "Cancel", "Anfrage wird beendet..." },
                { "Help", "Du kannst mich fragen den Robo Pi Eins links zu steuern." },
                { "Stop", "Tschüss" }
            });


            var localeSpeechFactory = new LocaleSpeechFactory(store);
            var locale = localeSpeechFactory.Create(skillRequest);

            return locale;
        }
    }
}
