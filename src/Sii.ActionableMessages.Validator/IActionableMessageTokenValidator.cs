using System.Threading.Tasks;

namespace Sii.ActionableMessages.Validator
{
    public interface IActionableMessageTokenValidator
    {
        Task<ActionableMessageTokenValidationResult> ValidateTokenAsync(string token, string targetServiceBaseUrl);
    }
}