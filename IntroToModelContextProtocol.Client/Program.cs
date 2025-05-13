using IntroToMicrosoftExtensionsAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

//setup the MCP client
var clientTransport = new SseClientTransport(new SseClientTransportOptions
{
    Name = "Restaurant",
    Endpoint = new Uri("http://localhost:5261/sse")
});

var mcpClient = await McpClientFactory.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

Console.WriteLine("The following tools are available from MCP:");
foreach (var tool in tools)
{
    Console.WriteLine($"  Tool: {tool.Name}");
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

Console.WriteLine("Say something to OpenAI and book your restaurant!");

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

    Console.WriteLine(chatResponse);
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
}
