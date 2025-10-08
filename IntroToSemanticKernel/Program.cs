using IntroToSemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using OpenTelemetry.Logs;
using VectorSearchUsingPostgres;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
#pragma warning disable SKEXP0001

AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnostics", true);
AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true);


var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var model = "gpt-4o";

var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

//where Semantic Kernel gets built
var semanticKernelBuilder = Kernel.CreateBuilder();

semanticKernelBuilder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b.AddSource("OpenAI.ChatClient");
        b.AddSource("Microsoft.SemanticKernel*");
    })
    .WithMetrics(b =>
    {
        b.AddMeter("OpenAI.ChatClient");
        b.AddMeter("Microsoft.SemanticKernel*");
    });

semanticKernelBuilder.Services.AddLogging(l =>
{
    l.SetMinimumLevel(LogLevel.Trace);
    l.AddSimpleConsole(c => c.SingleLine = true);
    l.AddOpenTelemetry(config =>
    {
        config.AddConsoleExporter();
    });
});
semanticKernelBuilder.AddOpenAIChatCompletion(model, openAIApiKey);
semanticKernelBuilder.Plugins.AddFromType<RestaurantBookingPlugin>();
semanticKernelBuilder.Services.AddSingleton(dbContext);

Kernel semanticKernel = semanticKernelBuilder.Build();

//where we get our chat client
var chatClient = semanticKernel.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory(new List<ChatMessageContent>
{
    new ChatMessageContent(AuthorRole.System, $"The current date/time in UTC is {DateTime.UtcNow}. You are a helpful restaurant reservation booking assistant.")
});
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

    chatHistory.Add(new ChatMessageContent(AuthorRole.User, line));

    var response = await chatClient.GetChatMessageContentsAsync(
        chatHistory,
        new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        },
        semanticKernel
    );
    var chatResponse = response.Last();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(chatResponse);
    Console.ResetColor();
    chatHistory.Add(chatResponse);
}

await postgresContainer.StopAsync();