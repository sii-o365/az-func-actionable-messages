using Microsoft.Azure.WebJobs.Description;
using System;

namespace Sii.ActionableMessages.Validator.WebJobs
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class ActionableMessageValidatorAttribute : Attribute
    {
    }
}
