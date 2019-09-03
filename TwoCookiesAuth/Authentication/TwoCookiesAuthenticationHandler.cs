using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace TwoCookiesAuth.Authentication
{
    public static class TwoCookiesAuthentication
    {
        public const string AuthenticationScheme = "TwoCookiesAuthentication";
        public const string TokenName = "access_token";
    }

    public class TwoCookiesAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string DefaultPayloadCookieName = "Identity.Payload";
        public const string DefaultSignatureCookieName = "Identity.Signature";

        public SecurityKey IssuerSigningKey { get; set; }
        public TimeSpan Expires { get; set; } = TimeSpan.FromDays(31);
        public string PayloadCookieName { get; set; } = DefaultPayloadCookieName;
        public string SignatureCookieName { get; set; } = DefaultSignatureCookieName;
    }

    public class TwoCookiesAuthenticationHandler : SignInAuthenticationHandler<TwoCookiesAuthenticationSchemeOptions>
    {
        private const string JwtIdKey = "jti";

        private readonly JwtSecurityTokenHandler _tokenHandler;

        public TwoCookiesAuthenticationHandler(IOptionsMonitor<TwoCookiesAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Cookies.TryGetValue(Options.PayloadCookieName, out var jwtHeaderAndPayload)
                && Request.Cookies.TryGetValue(Options.SignatureCookieName, out var jwtSignature))
            {
                Request.Headers["Authorization"] = $"Bearer {jwtHeaderAndPayload}.{jwtSignature}";
            }

            var result = await Context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                result.Principal.AddIdentity(new ClaimsIdentity(IdentityConstants.ApplicationScheme));
            }
            else
            {
                Response.Cookies.Delete(Options.PayloadCookieName);
                Response.Cookies.Delete(Options.SignatureCookieName);
            }

            return result;
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var jwtTokenId = Guid.NewGuid().ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(user.Identity, new[] { new Claim(JwtIdKey, jwtTokenId) }),
                Expires = DateTime.UtcNow.Add(Options.Expires),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(Options.IssuerSigningKey, SecurityAlgorithms.HmacSha256),
            };

            var token = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            Response.Cookies.Append(Options.PayloadCookieName, $"{token.EncodedHeader}.{token.EncodedPayload}", new CookieOptions
            {
                Expires = tokenDescriptor.Expires,
                HttpOnly = false,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });

            Response.Cookies.Append(Options.SignatureCookieName, token.RawSignature, new CookieOptions
            {
                Expires = tokenDescriptor.Expires,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });

            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(Options.PayloadCookieName);
            Response.Cookies.Delete(Options.SignatureCookieName);

            return Task.CompletedTask;
        }
    }
}
