using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GauntletSlack3.Api.Data;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        workerApplication.UseDefaultWorkerMiddleware();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<SlackDbContext>(options =>
            options.UseSqlServer(context.Configuration["ConnectionStrings:DefaultConnection"]));
    })
    .Build();

await host.RunAsync(); 