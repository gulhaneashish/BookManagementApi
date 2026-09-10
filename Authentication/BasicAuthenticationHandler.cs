using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BookStoreApi.Authentication;

public class BasicAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthService _authService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder,
        AuthService authService)
        : base(options, logger, encoder)
    {
        _authService = authService;
    }

    protected override async Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.NoResult();
        }

        try
        {
            var authorizationHeader =
    Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return AuthenticateResult.Fail(
                    "Authorization header is missing.");
            }

            var authHeader =
                AuthenticationHeaderValue.Parse(authorizationHeader);

            if (!authHeader.Scheme.Equals(
                    "Basic",
                    StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var credentialBytes =
                Convert.FromBase64String(
                    authHeader.Parameter!);

            var credentials =
                Encoding.UTF8.GetString(credentialBytes);

            var parts = credentials.Split(':', 2);

            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail(
                    "Invalid Basic Authentication credentials.");
            }

            var username = parts[0];
            var password = parts[1];

            var user = await _authService.ValidateUserAsync(
                username,
                password);

            if (user == null)
            {
                return AuthenticateResult.Fail(
                    "Invalid username or password.");
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                "Basic");

            var principal =
                new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(
                principal,
                "Basic");

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail(
                "Invalid Basic Authentication header.");
        }
    }
}