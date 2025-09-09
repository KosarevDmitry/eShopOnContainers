namespace Microsoft.eShopOnContainers.WebMVC;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the IoC container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews()
            .Services
            .AddAppInsight(Configuration)
            .AddHealthChecks(Configuration)
            .AddCustomMvc(Configuration)
            .AddDevspaces()
            .AddHttpClientServices(Configuration);

        IdentityModelEventSource.ShowPII = true;       // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII

        services.AddCustomAuthentication(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        var pathBase = Configuration["PATH_BASE"];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseStaticFiles();
        app.UseSession();

        WebContextSeed.Seed(app, env);

        // Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
        // Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = AspNetCore.Http.SameSiteMode.Lax });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("defaultError", "{controller=Error}/{action=Error}");
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }
}

static class ServiceCollectionExtensions
{
// что делает 
    public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // :: need to add Identity HealthChecks url 
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddUrlGroup(new Uri(configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });
        
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<AppSettings>(configuration);
        DebugLogger.Logger.Log("add DI for DistributedSessionStore and DataProtection");
        services.AddSession();
        DebugLogger.Logger.Log("add DI for MemoryDistributedCache");
        services.AddDistributedMemoryCache();

        if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString) // is false
        {   DebugLogger.Logger.Question("create Redis list `DataProtection-Keys`, it needs add DPConnectionString to config");
            services.AddDataProtection(opts =>
            {
                opts.ApplicationDiscriminator = "eshop.webmvc"; // симметричное шифрование
            })  
            .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys"); 
        }

        return services;
    }

    // Adds all Http client services
    public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        DebugLogger.Logger.Log("Add httpclients for various services");
        //register delegating handlers
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        services.AddTransient<HttpClientRequestIdDelegatingHandler>();

        //set 5 min as the lifetime for each HttpMessageHandler in the pool
        services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5)).AddDevspacesSupport();

        //add http client services
        services.AddHttpClient<IBasketService, BasketService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddDevspacesSupport();

        services.AddHttpClient<ICatalogService, CatalogService>()
                .AddDevspacesSupport();

        services.AddHttpClient<IOrderingService, OrderingService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>()
                .AddDevspacesSupport();

        // :: data received from IdentityServer
        //add custom application services
        services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

        return services;
    }


    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        DebugLogger.Logger.Log("IdentityUrl and CallBackUrl get from appsettings.json directly");
        var identityUrl = configuration.GetValue<string>("IdentityUrl");
        DebugLogger.Logger.Log("CallBackUrl get from appsettings.json directly, CallBackUrl is `IIS Express` url");
        var callBackUrl = configuration.GetValue<string>("CallBackUrl"); 
        var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

        // Add Authentication services          

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        { 
            // :: data send to IdentityServer
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = identityUrl.ToString();
            options.SignedOutRedirectUri = callBackUrl.ToString();
            options.ClientId = "mvc";
            options.ClientSecret = "secret";
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = false;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("orders");
            options.Scope.Add("basket");
            options.Scope.Add("webshoppingagg");
            options.Scope.Add("orders.signalrhub");
            options.Scope.Add("webhooks");
        });

        return services;
    }
}
