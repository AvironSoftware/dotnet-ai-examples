using IntroToMicrosoftExtensionsAI;
using Microsoft.Extensions.AI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var chatClient = ChatClientFactory.CreateChatClient();

var chatHistory = new List<ChatMessage>         //NOTE: there is a ChatMessage in OpenAI as well!
{
    new (ChatRole.System, "You are a helpful restaurant reservation booking assistant. " +
"No matter what the user says, always find out the current available restaurants and " +
"try to get them to book a restaurant because you is kinda pushy.")
};
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Say something to OpenAI and book your restaurant!");
Console.ResetColor();

var restaurantPlugin = new RestaurantBookingPlugin();

var chatOptions = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(restaurantPlugin.GetRestaurantsAvailableToBook)
    ],
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

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(chatResponse);
    Console.ResetColor();
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
}

