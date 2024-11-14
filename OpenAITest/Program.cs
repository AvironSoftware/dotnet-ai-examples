using OpenAI.Chat;

var openAIApiKey = "";
var client = new ChatClient("gpt-4o", openAIApiKey);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("You have a Southern accent and are friendly!")
};

Console.WriteLine("Say something to OpenAI!");

while (true)
{
    var line = Console.ReadLine();
    if (line == "exit")
    {
        break;
    }

    messages.Add(new UserChatMessage(line));

    var response = await client.CompleteChatAsync(messages);
    var chatResponse = response.Value.Content.Last().Text;

    Console.WriteLine(chatResponse);

    messages.Add(new AssistantChatMessage(chatResponse));
}