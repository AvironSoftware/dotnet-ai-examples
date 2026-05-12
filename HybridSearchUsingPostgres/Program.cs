using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using HybridSearchUsingPostgres;

// Agent with RAG/Search Integration Example
// Demonstrates an AI agent that uses TextSearchProvider to search a women's clothing catalog

var chatDeployment = "gpt-4.1";
var embeddingDeployment = "text-embedding-3-small";
var endpoint = new Uri(Environment.GetEnvironmentVariable("POSTGRES_TALK_AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("POSTGRES_TALK_AZURE_OPENAI_ENDPOINT environment variable is not set."));
var apiKey = Environment.GetEnvironmentVariable("POSTGRES_TALK_AZURE_OPENAI_API_KEY")
    ?? throw new InvalidOperationException("POSTGRES_TALK_AZURE_OPENAI_API_KEY environment variable is not set.");

Console.WriteLine("Agent with RAG/Search Demo");
Console.WriteLine("==========================");
Console.WriteLine();

// Start PostgreSQL with pgvector
Console.WriteLine("Starting PostgreSQL with pgvector...");
var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

try
{
    // Create the Azure OpenAI client
    var client = new AzureOpenAIClient(endpoint, new ApiKeyCredential(apiKey));
    var embeddingClient = client.GetEmbeddingClient(embeddingDeployment);

    // Create the search service
    var searchService = new HybridSearchService(dbContext, embeddingClient);

    // Configure the TextSearchProvider
    var textSearchOptions = new TextSearchProviderOptions
    {
        SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
        RecentMessageMemoryLimit = 6,
    };

    // Create the search adapter that bridges our HybridSearchService to the TextSearchProvider
    async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(
        string query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"  [search] query={query}");
        var results = await searchService.SearchAsync(query);
        Console.WriteLine($"  [search] => {results.Count} results");
        foreach (var r in results.Take(5))
        {
            Console.WriteLine($"             - {r.Name} (rrf={r.RrfScore:F4})");
        }
        return results.Select(r => new TextSearchProvider.TextSearchResult
        {
            SourceName = r.Name,
            SourceLink = $"doc://{r.Id}",
            Text = r.Contents
        });
    }

    // Create the AI agent with search capabilities
    AIAgent agent = client
        .GetChatClient(chatDeployment)
        .AsAIAgent(new ChatClientAgentOptions
        {
            Name = "KnowledgeAgent",
            ChatOptions = new()
            {
                Instructions = """
                    You are a helpful fashion assistant for a women's clothing store.
                    Answer questions using the provided context from our product catalog.
                    Always cite your sources when available.
                    If you don't find relevant information in the context, say so.
                    """
            },
            AIContextProviders = [new TextSearchProvider(
                SearchAdapter,
                textSearchOptions)]
        });

    // Demo queries
    var queries = new[]
    {
        "What dresses do you have?",
        "Do you have any summer tops?",
        "Tell me about your skirts"
    };

    foreach (var query in queries)
    {
        Console.WriteLine($"User: {query}");
        var response = await agent.RunAsync(query);
        Console.WriteLine($"Agent: {response.Text}");
        Console.WriteLine();
        Console.WriteLine("---");
        Console.WriteLine();
    }
}
finally
{
    await postgresContainer.StopAsync();
    Console.WriteLine("PostgreSQL container stopped.");
}
