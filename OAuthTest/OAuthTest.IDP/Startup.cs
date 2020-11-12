using System.Globalization;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OAuthTest.IDP.Repository;
using OAuthTest.IDP.Utils;

namespace OAuthTest.IDP
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            {
                services.AddLocalization(option =>
                {
                    option.ResourcesPath = "Resources";
                });

                services.AddMvcCore()
                    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                    options => options.ResourcesPath = "Resources");

                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("es-ES")
                };

                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.SetDefaultCulture(supportedCultures[0].Name);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.FallBackToParentCultures = true;
                    options.FallBackToParentUICultures = true;
                });

                services.AddSingleton<UserRepository>();

                ConfigureExternalAuthentications();

                ConfigureIdentityServer();

                // demonstration of replacing the default implementation with custom implementation
                services.Replace(ServiceDescriptor.Transient<ITokenService, CustomTokenService>());
                services.Replace(ServiceDescriptor.Transient<ITokenResponseGenerator, CustomTokenResponseGenerator>());
            }

            void ConfigureExternalAuthentications()
            {
                IdentityModelEventSource.ShowPII = true;
                services.AddAuthentication()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = Configuration["Secret:GoogleClientId"];
                    options.ClientSecret = Configuration["Secret:Secret:GoogleClientSecret"];
                })
                .AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "login";
                    options.ResponseType = "id_token";
                    options.SaveTokens = true;

                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Email);

                    options.CallbackPath = "/signin-idsrv";
                    options.SignedOutCallbackPath = "/signout-callback-idsrv";
                    options.RemoteSignOutPath = "/signout-idsrv";
                })
                .AddOpenIdConnect("manualidsrv", "IdentityServer Manual", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "login";
                    options.ResponseType = "id_token";
                    options.SaveTokens = true;

                    options.Configuration = new OpenIdConnectConfiguration
                    {
                        Issuer = "https://demo.identityserver.io",
                        //JwksUri = "https://demo.identityserver.io/.well-known/openid-configuration/jwks",
                        AuthorizationEndpoint = "https://demo.identityserver.io/connect/authorize",
                        TokenEndpoint = "https://demo.identityserver.io/connect/token",
                        UserInfoEndpoint = "https://demo.identityserver.io/connect/userinfo",
                        EndSessionEndpoint = "https://demo.identityserver.io/connect/endsession",
                    };

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudience = options.ClientId,

                        ValidateIssuer = true,
                        ValidIssuers = new[] { options.Authority },

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = ExternalOidcHelper.GetSigningKeysFromDemoIdentityServer(),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        RequireSignedTokens = true,
                    };

                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Email);

                    options.CallbackPath = "/signin-manualidsrv";
                    options.SignedOutCallbackPath = "/signout-callback-manualidsrv";
                    options.RemoteSignOutPath = "/signout-manualidsrv";
                })
                .AddOpenIdConnect("aad", "Azure AD", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "https://login.microsoftonline.com/common";
                    options.ClientId = Configuration["Secret:AzureClientId"];                    
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = "/signin-aad";
                    options.SignedOutCallbackPath = "/signout-callback-aad";
                    options.RemoteSignOutPath = "/signout-aad";

                    options.Scope.Add(IdentityServerConstants.StandardScopes.Email);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                }); 
            }

            void ConfigureIdentityServer()
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddTestUsers(Config.GetUsers())
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryApiScopes(Config.GetApiScopes())
                    .AddInMemoryClients(Config.GetClients())
                    //.AddClientStore<CustomClientStore>()                    
                    .AddProfileService<ProfileService>()
                    ;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

    public class CustomTokenService : DefaultTokenService
    {
        public CustomTokenService(IClaimsService claimsProvider, IReferenceTokenStore referenceTokenStore, ITokenCreationService creationService, IHttpContextAccessor contextAccessor, ISystemClock clock, IKeyMaterialService keyMaterialService, IdentityServerOptions options, ILogger<DefaultTokenService> logger) 
            : base(claimsProvider, referenceTokenStore, creationService, contextAccessor, clock, keyMaterialService, options, logger)
        {
        }

        public override async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            Token result = await base.CreateAccessTokenAsync(request);

            return result;
        }
    }

    public class CustomTokenResponseGenerator : TokenResponseGenerator
    {
        public CustomTokenResponseGenerator(ISystemClock clock, ITokenService tokenService, IRefreshTokenService refreshTokenService, IScopeParser scopeParser, IResourceStore resources, IClientStore clients, ILogger<TokenResponseGenerator> logger) : base(clock, tokenService, refreshTokenService, scopeParser, resources, clients, logger)
        {
        }

        public override async Task<TokenResponse> ProcessAsync(TokenRequestValidationResult request)
        {
            var result = await base.ProcessAsync(request);

            return result;
        }
    }
}
