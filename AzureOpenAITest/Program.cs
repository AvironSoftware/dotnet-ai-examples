
using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;

Console.WriteLine("Welcome to the Azure OpenAI Test!");

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
    Console.WriteLine("Say something to Azure OpenAI please!");
    var line = Console.ReadLine();

    if (line == "exit")
    {
        break;
    }

    messages.Add(new UserChatMessage(line));

    var response = await chatClient.CompleteChatAsync(messages);
    var chatResponse = response.Value.Content.Last().Text;

    Console.WriteLine(chatResponse);
    messages.Add(new AssistantChatMessage(chatResponse));
}