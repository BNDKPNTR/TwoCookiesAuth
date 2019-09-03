using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoCookiesAuth.Authentication;

namespace TwoCookiesAuth
{
    public static class AddAngularSpaExtensions
    {
        private const string BaseUrlKey = "base_url";
        private const string BearerTokenKey = "bearer_token";

        public static IApplicationBuilder UseAngularSpa(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                spa.UseSpaPrerendering(options =>
                {
                    options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.js";
                    options.ExcludeUrls = new[] { "/sockjs-node" };

                    options.SupplyData = async (context, data) =>
                    {
                        data[BaseUrlKey] = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
                        data[BearerTokenKey] = await context.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, TwoCookiesAuthentication.TokenName);
                    };
                });

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });

            return app;
        }
    }
}
