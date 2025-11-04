var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("Fingrid", (IServiceProvider serviceProvider, HttpClient client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = config["connStr:fingrid"];
    
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }

    client.BaseAddress = new Uri("https://data.fingrid.fi/api/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/fingrid", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Fingrid");

    try
    {
        var response = await client.GetAsync("datasets/363/");
        response.EnsureSuccessStatusCode();
        return Results.Ok(await response.Content.ReadAsStringAsync());
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Failed to fetch data: {ex.Message}", statusCode: 502);
    }
})
.WithName("FingridData");

app.Run();