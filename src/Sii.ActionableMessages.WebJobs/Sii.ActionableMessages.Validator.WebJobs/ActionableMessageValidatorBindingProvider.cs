using Microsoft.Azure.WebJobs.Host.Bindings;
using System.Threading.Tasks;

namespace Sii.ActionableMessages.Validator.WebJobs
{
    internal class ActionableMessageValidatorBindingProvider : IBindingProvider
    {
        public async Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            await Task.Yield();

            if (context.Parameter.ParameterType != typeof(ActionableMessageTokenValidationResult))
            {
                throw new UnsuportedBindingTypeException();
            }

            return new ActionableMessageValidatorBinding();
        }
    }
}
