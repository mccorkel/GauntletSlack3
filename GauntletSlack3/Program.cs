using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GauntletSlack3;
using GauntletSlack3.Services;
using GauntletSlack3.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with base address and CORS
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress)
    };
    
    // Add default headers for all requests
    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    
    // Add CORS headers
    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
    
    return httpClient;
});

// Add auth message handler
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddHttpClient("AuthenticatedClient")
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// Register services
builder.Services.AddScoped<IMessageService, MessageService>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<RealTimeService>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<IUserStateService>(sp => sp.GetRequiredService<UserStateService>());
builder.Services.AddMemoryCache();

// Auth configuration
builder.Services.AddAuthorizationCore();
builder.Services.AddMsalAuthentication(options =>
{
    var authentication = options.ProviderOptions.Authentication;
    builder.Configuration.Bind("AzureAd", authentication);
    
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("email");
    
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
    
    options.UserOptions.NameClaim = "name";
    options.ProviderOptions.LoginMode = "redirect";
});

// Add logging configuration
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(builder.HostEnvironment.IsProduction() 
        ? LogLevel.Information 
        : LogLevel.Debug);
});

// Add before other services
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
