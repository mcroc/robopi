using System;
using System.Collections.Generic;
using System.IO;
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
            var locale = SetupLanguages(skillRequest);

            SkillResponse response = null;

            if (skillRequest == null)
            {
                response = ResponseBuilder.Tell("SkillRequest konnte nicht gelesen werden.");
                response.Response.ShouldEndSession = false;
                
                return new OkObjectResult(response);
            }

            var requestType = skillRequest.GetRequestType();
            

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Wilkommen beim DevOpenSpace!");
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                var intentName = intentRequest?.Intent.Name;
                _logger.LogInformation(intentName);
                if (!string.IsNullOrEmpty(intentName))
                {
                    var intentMessage = _intentService.GetMessage(intentName);
                    _logger.LogInformation(intentMessage.Message);

                    var serviceBusMessage = new ServiceBusMessage(intentMessage.Intent.ToString());
                    string connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");

                    if (!string.IsNullOrEmpty(connectionString) && intentMessage.RoboPiNumber > 0)
                    {
                        ServiceBusClient client = new ServiceBusClient(connectionString);
                        var sender = client.CreateSender($"robopicarnumber{intentMessage.RoboPiNumber}");
                        await sender.SendMessageAsync(serviceBusMessage, new CancellationToken());
                    }
                    else if (intentRequest.Intent.Name == "AMAZON.CancelIntent")
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

                    response = ResponseBuilder.Tell(intentMessage.Message);
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

        public static ILocaleSpeech SetupLanguages(SkillRequest skillRequest)
        {
            var store = new DictionaryLocaleSpeechStore();
            store.AddLanguage("en", new Dictionary<string, object>
            {
                { "Welcome", "Welcome to the AppConsult skill!" },
                { "LastPosts", "The title of the last article is {0}" },
                { "Cancel", "I'm cancelling the request..." },
                { "Help", "You can ask me, for example, which is the last article." },
                { "Stop", "Goodbye!" }
            });

            store.AddLanguage("it", new Dictionary<string, object>
            {
                { "Welcome", "Benvenuti in Windows AppConsult!" },
                { "LastPosts", "Il titolo dell'ultimo articolo è {0}" },
                { "Cancel", "Sto annullando la richiesta..." },
                { "Help", "Puoi chiedermi, ad esempio, qual è l'ultimo articolo. " },
                { "Stop", "Alla prossima!" }
            });


            var localeSpeechFactory = new LocaleSpeechFactory(store);
            var locale = localeSpeechFactory.Create(skillRequest);

            return locale;
        }
    }
}
