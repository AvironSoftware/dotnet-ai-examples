using IntroToMicrosoftExtensionsAI;
using Microsoft.Extensions.AI;
using OpenAI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var chatClient = ChatClientFactory.CreateChatClient();

var chatHistory = new List<ChatMessage>         //NOTE: there is a ChatMessage in OpenAI as well!
{
    new (ChatRole.System, "You are a helpful restaurant reservation booking assistant.")
};
Console.WriteLine("Say something to OpenAI and book your restaurant!");

var restaurantPlugin = new RestaurantBookingPlugin();

var chatOptions = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(restaurantPlugin.GetRestaurantsAvailableToBook)
    ]
};

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

