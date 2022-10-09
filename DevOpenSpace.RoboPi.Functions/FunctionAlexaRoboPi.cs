using System;
using System.IO;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using DevOpenSpace.RoboPi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                    response = ResponseBuilder.Tell(intentMessage.Message);
                    response.Response.ShouldEndSession = false;
                }
            }

            return new OkObjectResult(response);
        }
    }
}
