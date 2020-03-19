using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sii.ActionableMessages.Validator.WebJobs
{
    internal class ActionableMessageValidatorBinding : IBinding
    {
        public bool FromAttribute => true;

        public async Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            await Task.Yield();
            return new ActionableMessageValidationResultProvider((ActionableMessageTokenValidationResult)value);
        }

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            var validator = new ActionableMessageTokenValidator();
            ActionableMessageTokenValidationResult result = null;

            if (context.BindingData.TryGetValue("$request", out var request))
            {
                var req = (HttpRequest)request;
                if (req.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var token = authHeader.FirstOrDefault()?.Substring("Bearer ".Length)?.Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var serviceUrl = $"{req.Scheme}://{req.Host}";
                        result = await validator.ValidateTokenAsync(token, serviceUrl);
                    }
                }
            }

            return await BindAsync(result, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();

        private class ActionableMessageValidationResultProvider : IValueProvider
        {
            private ActionableMessageTokenValidationResult _value;

            public ActionableMessageValidationResultProvider(ActionableMessageTokenValidationResult value)
            {
                _value = value;
            }

            public Type Type => typeof(ActionableMessageTokenValidationResult);

            public Task<object> GetValueAsync() => Task.FromResult<object>(_value);

            public string ToInvokeString() => _value?.ToString();
        }
    }
}
