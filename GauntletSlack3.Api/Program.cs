using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Api.Data;
using Microsoft.Azure.WebPubSub.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.Web.BrowserLink;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<SlackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add WebPubSub
builder.Services.AddWebPubSub();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseBrowserLink();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Map WebPubSub hub
app.MapWebPubSubHub<GauntletSlack3.Api.Hubs.UserStatusHub>("/api/hubs/userstatus");

app.Run(); 