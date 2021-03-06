﻿using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OAuthTest.Students
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            {
                services.Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

                services.AddLocalization(option =>
                {
                    option.ResourcesPath = "Resources";
                });

                var supportedCultures = new[]
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("es-ES")
                };

                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

                ConfigureAuthorization();
                ConfigureAuthentication();

                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                    options => options.ResourcesPath = "Resources");
            }

            void ConfigureAuthorization()
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("UKStudentOnly",
                        policyBuilder =>
                        {
                            policyBuilder.RequireAuthenticatedUser();
                            policyBuilder.RequireClaim(Constants.CustomClaimTypes.Country, "UK");
                        });
                });
            }

            void ConfigureAuthentication()
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options =>
                {
                    options.AccessDeniedPath = "/AccessDenied";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = Constants.Urls.IdentityServerProviderUrl;
                    options.ClientId = Constants.Clients.Students;
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    options.Scope.Add("country");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("custom_ids");
                    options.Scope.Add(Constants.Clients.ApiStudents);
                    options.Scope.Add(Constants.Clients.ApiTeachers);
                    options.Scope.Add(Constants.Clients.ApiCourses);

                    options.SaveTokens = true;
                    options.ClientSecret = Constants.Secrets.SharedSecret;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("idp");

                    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
                    options.ClaimActions.MapUniqueJsonKey(Constants.CustomClaimTypes.Country, Constants.CustomClaimTypes.Country);
                    options.ClaimActions.MapUniqueJsonKey("custom_user_id", "custom_user_id");
                    options.ClaimActions.MapUniqueJsonKey("custom_company_id", "custom_company_id");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        RoleClaimType = JwtClaimTypes.Role
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = ctx =>
                        {
                            ctx.ProtocolMessage.UiLocales = Thread.CurrentThread.CurrentUICulture.Name;
                            ctx.ProtocolMessage.State = $"Request Sent at {DateTime.UtcNow}";
                            return Task.CompletedTask;
                        },
                        OnAuthorizationCodeReceived = ctx =>
                        {
                            Console.WriteLine("OnAuthorizationCodeReceived");
                            return Task.CompletedTask;
                        },
                        OnUserInformationReceived = ctx =>
                        {
                            Console.WriteLine("OnUserInformationReceived ");
                            return Task.CompletedTask;
                        },
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "AccessDenied",
                    pattern: "AccessDenied",
                    defaults: new { controller = "Home", action = "AccessDenied" });

                endpoints.MapDefaultControllerRoute();                
            });
        }
    }
}
