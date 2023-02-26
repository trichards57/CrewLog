using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace BlazorApp.Client.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IConfiguration config;

        private readonly HttpClient http;

        public AuthStateProvider(IConfiguration config, IWebAssemblyHostEnvironment environment)
        {
            this.config = config;
            this.http = new HttpClient { BaseAddress = new Uri(environment.BaseAddress) };
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var authDataUrl = config.GetValue("StaticWebAppsAuthentication:AuthenticationDataUrl", "/.auth/me");
                var data = await http.GetFromJsonAsync<ClientPrincipal>(authDataUrl);

                var principal = data with { UserRoles = data.UserRoles.Except(new string[] { "anonymous" }, StringComparer.OrdinalIgnoreCase) };

                if (!principal.UserRoles.Any())
                    return new AuthenticationState(new ClaimsPrincipal());

                var identity = new ClaimsIdentity(principal.IdentityProvider);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
                identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        private readonly record struct AuthenticationData
        {
            public ClientPrincipal ClientPrincipal { get; init; }
        }

        private readonly record struct ClientPrincipal
        {
            [JsonPropertyName("identityProvider")]
            public string IdentityProvider { get; init; }

            [JsonPropertyName("userDetails")]
            public string UserDetails { get; init; }

            [JsonPropertyName("userId")]
            public string UserId { get; init; }

            [JsonPropertyName("userRoles")]
            public IEnumerable<string> UserRoles { get; init; }
        }
    }
}
