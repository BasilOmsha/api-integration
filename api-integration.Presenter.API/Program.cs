using System.Diagnostics;
using System.Reflection;
using api_integration.Presenter.API.src;
using api_integration.Presenter.API.src.RouteTransformer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SpinCaseTransformer()));
});

// Register all project DIs
builder.Services.DI(builder.Configuration);

builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("isSuccess", "false");
        context.ProblemDetails.Extensions.TryAdd("isFailure", "true");
        context.ProblemDetails.Extensions.TryAdd("value", null);

    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API-Integration",
        Description = "Integrating Open Data",
    });

    //Show XML comments on swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API-Integration v1");
    });
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("API-Integration")
            .WithTheme(ScalarTheme.Kepler)
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json") // Swagger document to get xml comments
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();
app.Run();