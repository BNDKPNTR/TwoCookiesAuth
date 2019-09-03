using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoCookiesAuth.Authentication;
using TwoCookiesAuth.Dal;
using TwoCookiesAuth.Dal.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwoCookiesAuthenticationExtensions
    {
        public static IServiceCollection AddTwoCookiesAppIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<TwoCookiesAppDbContext>();

            var jwtSigningKey = configuration.GetValue<string>("JwtSigningKey");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey));

            services.AddAuthentication()
                .AddScheme<TwoCookiesAuthenticationSchemeOptions, TwoCookiesAuthenticationHandler>(TwoCookiesAuthentication.AuthenticationScheme, options =>
                {
                    options.IssuerSigningKey = signingKey;
                    options.Expires = TimeSpan.FromDays(31);
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.IssuerSigningKey = signingKey;
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.RequireExpirationTime = true;
                    options.TokenValidationParameters.RequireSignedTokens = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.ForwardAuthenticate = TwoCookiesAuthentication.AuthenticationScheme;
                options.ForwardSignIn = TwoCookiesAuthentication.AuthenticationScheme;
                options.ForwardSignOut = TwoCookiesAuthentication.AuthenticationScheme;
            });

            return services;
        }
    }
}
