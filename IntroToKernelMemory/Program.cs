using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

var memory = new KernelMemoryBuilder()
    .Configure(builder => builder.Services.AddLogging(l =>
    {
        l.SetMinimumLevel(LogLevel.Trace);
        l.AddSimpleConsole(c => c.SingleLine = true);
    }))
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build<MemoryServerless>();

var document = new Document("doc01")
    .AddFile("file1-Wikipedia-Carbon.txt")
    .AddFile("file2-Wikipedia-Moon.txt");

await memory.ImportDocumentAsync(document);

while (!await memory.IsDocumentReadyAsync("doc01"))
{
    Console.WriteLine("Waiting for memory ingestion to complete...");
    await Task.Delay(TimeSpan.FromMilliseconds(1500));
}

Console.WriteLine("Ask me something or type exit to quit:");

while (true)
{
    var question = Console.ReadLine();
    if (question == "exit")
    {
        break;
    }

    var response = await memory.AskAsync(question);
    Console.WriteLine($"* Ask response: {response}");
}
