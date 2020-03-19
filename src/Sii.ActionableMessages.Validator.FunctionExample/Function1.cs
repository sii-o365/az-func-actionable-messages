using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sii.ActionableMessages.Validator.WebJobs;

namespace Sii.ActionableMessages.Validator.FunctionExample
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
#pragma warning disable RCS1163 // Unused parameter.
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
#pragma warning restore RCS1163 // Unused parameter.
            [ActionableMessageValidator]ActionableMessageTokenValidationResult tokenValidation,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(1);
        }
    }
}
