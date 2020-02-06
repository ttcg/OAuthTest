using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ExternalClient.IDP
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {            
            HostingEnvironment = env;
        }

        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            {
                services.AddMvc();

                ConfigureExternalAuthentications();

                ConfigureIdentityServer();
            }

            void ConfigureExternalAuthentications()
            {
                services.AddAuthentication()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "319143812640-qntvt2snefo4hmh43fiuj2j1a7ju5l0c.apps.googleusercontent.com";
                    options.ClientSecret = "Oj9DaTHFrhqe7xoxK_G1LWeO";
                });
            }

            void ConfigureIdentityServer()
            {
                services.AddIdentityServer()                    
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddCustomSiginingCertificate(HostingEnvironment);
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
