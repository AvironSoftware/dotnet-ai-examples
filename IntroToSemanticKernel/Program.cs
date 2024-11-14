using IntroToSemanticKernel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

var openAIApiKey = "";  //add your OpenAI API key here
var model = "gpt-4o";

//where Semantic Kernel gets built
var semanticKernelBuilder = Kernel.CreateBuilder();
semanticKernelBuilder.AddOpenAIChatCompletion(model, openAIApiKey);
semanticKernelBuilder.Plugins.AddFromObject(new RestaurantBookingPlugin());

Kernel semanticKernel = semanticKernelBuilder.Build();

//where we get our chat client
var chatClient = semanticKernel.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory(new List<ChatMessageContent>
{
    new ChatMessageContent(AuthorRole.System, "You are a helpful restaurant reservation booking assistant.")
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