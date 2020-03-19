using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Sii.ActionableMessages.Validator
{
    /// <summary>
    /// Class to validate an actionable message token.
    /// </summary>
    public class ActionableMessageTokenValidator : IActionableMessageTokenValidator
    {
        /// <summary>
        /// The clock skew to apply when validating times in a token.
        /// </summary>
        private const int _tokenTimeValidationClockSkewBufferInMinutes = 5;

        /// <summary>
        /// The OpenID configuration data retriever.
        /// </summary>
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        /// <summary>
        /// Constructor of the <see cref="ActionableMessageTokenValidator"/> class.
        /// </summary>
        public ActionableMessageTokenValidator()
        {
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                O365OpenIdConfiguration.MetadataUrl,
                new OpenIdConnectConfigurationRetriever());
        }

        /// <inheritdoc />
        public async Task<ActionableMessageTokenValidationResult> ValidateTokenAsync(
            string token,
            string targetServiceBaseUrl)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("token is null or empty.", "token");
            }

            if (string.IsNullOrEmpty(targetServiceBaseUrl))
            {
                throw new ArgumentException("url is null or empty.", "targetServiceBaseUrl");
            }

            CancellationToken cancellationToken;
            OpenIdConnectConfiguration o365OpenIdConfig = await _configurationManager.GetConfigurationAsync(cancellationToken);
            ClaimsPrincipal claimsPrincipal;
            ActionableMessageTokenValidationResult result = new ActionableMessageTokenValidationResult();

            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { O365OpenIdConfiguration.TokenIssuer },
                ValidateAudience = true,
                ValidAudiences = new[] { targetServiceBaseUrl },
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(_tokenTimeValidationClockSkewBufferInMinutes),
                RequireSignedTokens = true,
                IssuerSigningKeys = o365OpenIdConfig.SigningKeys,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;

            try
            {
                // This will validate the token's lifetime and the following claims:
                // 
                // iss
                // aud
                //
                claimsPrincipal = tokenHandler.ValidateToken(token, parameters, out validatedToken);
            }
            catch (SecurityTokenSignatureKeyNotFoundException ex)
            {
                result.Exception = ex;
                return result;
            }
            catch (SecurityTokenExpiredException ex)
            {
                result.Exception = ex;
                return result;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                result.Exception = ex;
                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }

            if (claimsPrincipal == null)
            {
                result.Exception = new InvalidOperationException("Identity not found in the token");
                return result;
            }

            ClaimsIdentity identity = claimsPrincipal.Identities.OfType<ClaimsIdentity>().FirstOrDefault();
            if (identity == null)
            {
                result.Exception = new InvalidOperationException("Claims not found in the token.");
                return null;
            }

            if (!string.Equals(GetClaimValue(identity, "appid"), O365OpenIdConfiguration.AppId, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            result.ValidationSucceeded = true;
            result.Sender = GetClaimValue(identity, "sender");

            // Get the value of the "sub" claim. Passing in "sub" will not return a value because the TokenHandler
            // maps "sub" to ClaimTypes.NameIdentifier. More info here
            // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/415.
            result.ActionPerformer = GetClaimValue(identity, ClaimTypes.NameIdentifier);

            return result;
        }

        /// <summary>
        /// Gets the value of a claim type from the identity.
        /// </summary>
        /// <param name="identity">The identity to read the claim from.</param>
        /// <param name="claimType">The claim type.</param>
        /// <returns>The value of the claim if it exists; else is null.</returns>
        private static string GetClaimValue(ClaimsIdentity identity, string claimType)
        {
            Claim claim = identity.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
