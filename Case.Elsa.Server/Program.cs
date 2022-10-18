using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.SqlServer;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddElsa(
    elsaOptionsBuilder => elsaOptionsBuilder
        .UseEntityFrameworkPersistence(
            dbContextoptionsBuilder => dbContextoptionsBuilder
                .UseSqlServer(webApplicationBuilder.Configuration
                    .GetConnectionString("Elsa")))
        .AddHttpActivities(webApplicationBuilder.Configuration
            .GetSection("Elsa")
            .GetSection("Server")
            .Bind)
        .AddQuartzTemporalActivities()
        .AddJavaScriptActivities()
        .AddWorkflowsFrom<Program>());

webApplicationBuilder.Services.AddElsaApiEndpoints();

// Allow arbitrary client browser apps to access the API.
// In a production environment, make sure to allow only origins you trust.
webApplicationBuilder.Services.AddCors(
    corsOptions => corsOptions.AddDefaultPolicy(
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .WithExposedHeaders("Content-Disposition")));

var webApplication = webApplicationBuilder.Build();

if (webApplicationBuilder.Environment.IsDevelopment())
{
    webApplication.UseDeveloperExceptionPage();
}

webApplication
    .UseCors()
    .UseHttpActivities()
    .UseRouting()
    .UseEndpoints(endpointRouterBuilder =>
    {
        endpointRouterBuilder.MapControllers();
    });

webApplication.Run();