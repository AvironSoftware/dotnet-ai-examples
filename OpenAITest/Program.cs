using OpenAI.Responses;

#pragma warning disable OPENAI001

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new ResponsesClient(
    model: "gpt-4o",
    apiKey: apiKey);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("I'm OpenAI, ask me anything!");
Console.ResetColor();

string? previousResponseId = null;

while (true)
{
    var line = Console.ReadLine();
    if (line == "exit")
    {
        break;
    }

    var options = new CreateResponseOptions
    {
        PreviousResponseId = previousResponseId
    };
    //options.Tools.Add(ResponseTool.CreateWebSearchTool());
    options.InputItems.Add(ResponseItem.CreateUserMessageItem(line!));

    var result = await client.CreateResponseAsync(options);
    var response = result.Value;
    previousResponseId = response.Id;

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine($"Input tokens: {response.Usage.InputTokenCount}");
    Console.WriteLine($"Output tokens: {response.Usage.OutputTokenCount}");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(response.GetOutputText());
    Console.ResetColor();
}
