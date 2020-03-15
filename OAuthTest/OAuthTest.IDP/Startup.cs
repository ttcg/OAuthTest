﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OAuthTest.IDP.Repository;

namespace OAuthTest.IDP
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            {
                services.AddMvc();
                services.AddSingleton<UserRepository>();

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
                    .AddDeveloperSigningCredential()
                    .AddTestUsers(Config.GetUsers())
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    //.AddClientStore<CustomClientStore>()                    
                    .AddProfileService<ProfileService>()
                    ;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}