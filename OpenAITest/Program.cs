using OpenAI.Chat;

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new ChatClient("gpt-4o", openAIApiKey);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("You have a Southern accent and are friendly!")
};

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Say something to OpenAI!");
Console.ResetColor();

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

    var usage = response.Value.Usage;
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Input tokens: " + usage.InputTokenCount);
    Console.WriteLine("Output tokens: " + usage.OutputTokenCount);
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(chatResponse);
    Console.ResetColor();

    messages.Add(new AssistantChatMessage(chatResponse));
}