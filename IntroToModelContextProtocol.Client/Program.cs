using IntroToMicrosoftExtensionsAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

//setup the MCP client
var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("http://localhost:5261/mcp")
});

var mcpClient = await McpClient.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("The following tools are available from MCP:");
Console.ResetColor();
foreach (var tool in tools)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"  Tool: {tool.Name}");
    Console.ResetColor();
}

var chatClient = ChatClientFactory.CreateChatClient();
var chatHistory = new List<ChatMessage>         //NOTE: there is a ChatMessage in OpenAI as well!
{
    new (ChatRole.System, "You are a helpful restaurant reservation booking assistant.")
};
var chatOptions = new ChatOptions
{
    Tools = [
        ..tools
    ]
};

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

    chatHistory.Add(new ChatMessage(ChatRole.User, line));

    var response = await chatClient.GetResponseAsync(chatHistory, chatOptions);
    var chatResponse = response.Text;

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(chatResponse);
    Console.ResetColor();
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
}
