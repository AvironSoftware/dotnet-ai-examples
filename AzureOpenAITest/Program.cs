
using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Welcome to the Azure OpenAI Test!");
Console.ResetColor();

var deploymentName = "";
var endpointUrl = "";
var key = "";

var client = new AzureOpenAIClient(
    new Uri(endpointUrl),
    new ApiKeyCredential(key)
);
var chatClient = client.GetChatClient(deploymentName);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("You have a Southern accent and are friendly!")
};

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Say something to Azure OpenAI please!");
    Console.ResetColor();
    var line = Console.ReadLine();

    if (line == "exit")
    {
        break;
    }

    messages.Add(new UserChatMessage(line));

    var response = await chatClient.CompleteChatAsync(messages);
    var chatResponse = response.Value.Content.Last().Text;

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(chatResponse);
    Console.ResetColor();
    messages.Add(new AssistantChatMessage(chatResponse));
}