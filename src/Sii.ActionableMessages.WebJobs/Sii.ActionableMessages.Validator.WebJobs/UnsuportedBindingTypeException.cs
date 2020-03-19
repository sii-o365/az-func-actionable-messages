using System;

namespace Sii.ActionableMessages.Validator.WebJobs
{
    public class UnsuportedBindingTypeException : Exception
    {
        public UnsuportedBindingTypeException()
            : base($"ActionableMessageValidator supports only {nameof(ActionableMessageTokenValidationResult)} binding.")
        {
        }
    }
}
