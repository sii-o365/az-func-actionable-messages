using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Sii.ActionableMessages.Validator.WebJobs.Startup))]
namespace Sii.ActionableMessages.Validator.WebJobs
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<MessageValidatorExtension>();
        }
    }
}
