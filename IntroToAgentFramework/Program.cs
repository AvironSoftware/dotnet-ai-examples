using IntroToAgentFramework;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;

var client = ChatClientFactory.CreateChatClient().AsIChatClient();
var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();

var sourceName = Guid.NewGuid().ToString("N");
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(sourceName)
    .AddConsoleExporter()
    .Build();

var services = new ServiceCollection();
services.AddSingleton(postgresContainer.GetDbContext());
services.AddScoped<RestaurantBookingService>();
services.AddScoped<AIAgent>(sp =>
{
    var db = sp.GetRequiredService<RestaurantDbContext>();
    var service = new RestaurantBookingService(db);

    return client.AsAIAgent(
        name: "RestaurantBooker",
        instructions: $"""
                      The current date/time in UTC is {DateTime.UtcNow}.
                      You are a helpful restaurant reservation booking assistant.
                      No matter what the user says, always find out the current available restaurants and
                      try to get them to book a restaurant because you is kinda pushy.
                      """,
        tools: service.AsAITools()
    )
    .AsBuilder()
    .UseOpenTelemetry(sourceName)
    .Build();
});

var serviceProvider = services.BuildServiceProvider();
var agent = serviceProvider.GetRequiredService<AIAgent>();
var session = await agent.GetNewSessionAsync();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Say something to OpenAI and book your restaurant!");
Console.ResetColor();

while (true)
{
    var line = Console.ReadLine();
    if (line == "exit")
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    var response = await agent.RunAsync(line, session);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(response.Text);
    Console.ResetColor();
}

await postgresContainer.StopAsync();