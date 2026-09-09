using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MAPCsDashboard.Api.Authentication;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<ApiKeyOptions> apiKeyOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(schemeOptions, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var suppliedValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var supplied = suppliedValues.ToString();
        var configured = apiKeyOptions.Value.Key;
        if (!SecureEquals(supplied, configured))
        {
            return Task.FromResult(AuthenticateResult.Fail("La clave de API no es válida."));
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "usuario-api")],
            ApiKeyAuthenticationDefaults.Scheme);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }

    private static bool SecureEquals(string supplied, string configured)
    {
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
        var configuredBytes = Encoding.UTF8.GetBytes(configured);
        return suppliedBytes.Length == configuredBytes.Length
            && CryptographicOperations.FixedTimeEquals(suppliedBytes, configuredBytes);
    }
}

