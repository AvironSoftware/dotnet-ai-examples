using IntroToSemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using VectorSearchUsingPostgres;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var model = "gpt-4o";

var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

//where Semantic Kernel gets built
var semanticKernelBuilder = Kernel.CreateBuilder();
semanticKernelBuilder.Services.AddLogging(l =>
{
    l.SetMinimumLevel(LogLevel.Trace);
    l.AddSimpleConsole(c => c.SingleLine = true);
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
Console.WriteLine("Say something to OpenAI and book your restaurant!");

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

    Console.WriteLine(chatResponse);
    chatHistory.Add(chatResponse);
}

await postgresContainer.StopAsync();