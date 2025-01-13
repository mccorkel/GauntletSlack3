using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using GauntletSlack3.Api.Hubs;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using GauntletSlack3.Api.Services;
using GauntletSlack3.Api.Services.Interfaces;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Serilog.AspNetCore;
using Serilog.Sinks.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GauntletSlack3 API",
        Version = "v1",
        Description = "API for GauntletSlack3 chat application"
    });
    
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token")
            }
        }
    });
});

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
    options.StreamBufferCapacity = 10;
});

// Add CORS for Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
                "https://localhost:7229",  // Client HTTPS
                "http://localhost:5258",   // Client HTTP
                "https://jolly-sand-0f41c5a1e.4.azurestaticapps.net"  // Production
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)  // Allow any origin for preflight
            .WithExposedHeaders("*");
    });
});

// Add DbContext
builder.Services.AddDbContext<SlackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
);

builder.Services.AddHostedService<BackgroundMessageProcessor>();
builder.Services.AddScoped<IMessageQueueService, MessageQueueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Move CORS before HTTPS redirection
app.UseCors();

// HTTPS redirection after CORS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapHub<UserStatusHub>("/hubs/userstatus");

// Add WebSocket options
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});

app.Run(); 