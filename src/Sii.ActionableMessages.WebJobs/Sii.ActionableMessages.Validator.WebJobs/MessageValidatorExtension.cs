using Microsoft.Azure.WebJobs.Host.Config;

namespace Sii.ActionableMessages.Validator.WebJobs
{
    internal class MessageValidatorExtension : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<ActionableMessageValidatorAttribute>();
            rule.Bind(new ActionableMessageValidatorBindingProvider());
        }
    }
}
