using System.ClientModel;
using System.ComponentModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using AgenticSearchUsingPostgres;

// Agentic Search Example
// Unlike the hybrid-search sample, the agent autonomously decides when/what to search,
// evaluates results, and may refine queries with multiple search calls.

var chatDeployment = "gpt-4.1";
var embeddingDeployment = "text-embedding-3-small";
var endpoint = new Uri(Environment.GetEnvironmentVariable("POSTGRES_TALK_AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("POSTGRES_TALK_AZURE_OPENAI_ENDPOINT environment variable is not set."));
var apiKey = Environment.GetEnvironmentVariable("POSTGRES_TALK_AZURE_OPENAI_API_KEY")
    ?? throw new InvalidOperationException("POSTGRES_TALK_AZURE_OPENAI_API_KEY environment variable is not set.");

Console.WriteLine("Agentic Search Demo");
Console.WriteLine("====================");
Console.WriteLine("The agent autonomously decides when and what to search.");
Console.WriteLine();

// Start PostgreSQL with pgvector
Console.WriteLine("Starting PostgreSQL with pgvector...");
var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

try
{
    var client = new AzureOpenAIClient(endpoint, new ApiKeyCredential(apiKey));
    var embeddingClient = client.GetEmbeddingClient(embeddingDeployment);

    var searchService = new HybridSearchService(dbContext, embeddingClient);

    // Multiple function tools give the agent flexibility to search in different ways
    [Description("Search the product catalog using a natural language query. Returns matching products with relevance scores.")]
    async Task<string> SearchProducts([Description("Natural language search query")] string query)
    {
        Console.WriteLine($"  [tool] SearchProducts(query=\"{query}\")");
        var results = await searchService.SearchAsync(query, limit: 5);
        if (results.Count == 0)
        {
            Console.WriteLine("  [tool] => No products found.");
            return "No products found matching the query.";
        }
        Console.WriteLine($"  [tool] => {results.Count} results");
        foreach (var r in results)
            Console.WriteLine($"           - [ID:{r.Id}] {r.Name} (rrf={r.RrfScore:F4})");

        return string.Join("\n", results.Select(r =>
            $"[ID:{r.Id}] {r.Name} (score: {r.RrfScore:F4})\n  {r.Contents[..Math.Min(150, r.Contents.Length)]}...\n"));
    }

    [Description("Get full details for a specific product by its ID")]
    async Task<string> GetProductDetails([Description("The product ID")] int productId)
    {
        Console.WriteLine($"  [tool] GetProductDetails(productId={productId})");
        var product = await searchService.GetProductByIdAsync(productId);
        if (product is null)
        {
            Console.WriteLine($"  [tool] => Product {productId} not found.");
            return $"Product {productId} not found.";
        }
        Console.WriteLine($"  [tool] => {product.Name}");
        return $"{product.Name}\n{product.Contents}";
    }

    [Description("Search products by category or subcategory (e.g., 'Dresses', 'Tops', 'Skirts', 'Jeans')")]
    async Task<string> SearchByCategory([Description("Category or subcategory to search")] string category)
    {
        Console.WriteLine($"  [tool] SearchByCategory(category=\"{category}\")");
        var results = await searchService.SearchByCategoryAsync(category, limit: 5);
        if (results.Count == 0)
        {
            Console.WriteLine($"  [tool] => No products in category '{category}'.");
            return $"No products found in category '{category}'.";
        }
        Console.WriteLine($"  [tool] => {results.Count} results");
        foreach (var r in results)
            Console.WriteLine($"           - [ID:{r.Id}] {r.Name}");

        return string.Join("\n", results.Select(r =>
            $"[ID:{r.Id}] {r.Name}\n  {r.Contents[..Math.Min(150, r.Contents.Length)]}...\n"));
    }

    AIAgent agent = client
        .GetChatClient(chatDeployment)
        .AsAIAgent(
            instructions: """
                You are an expert fashion consultant for a women's clothing store.
                You have access to our product catalog through search tools.
                When helping customers:
                1. Search for relevant products using the most appropriate tool
                2. If the initial search doesn't find what you need, refine your query and search again
                3. Use GetProductDetails to get full information on promising items
                4. For broad requests, search by category first, then refine
                5. Combine results from multiple searches to give comprehensive recommendations
                6. Always explain why you're recommending specific items
                """,
            name: "FashionConsultant",
            tools: [
                AIFunctionFactory.Create(SearchProducts),
                AIFunctionFactory.Create(GetProductDetails),
                AIFunctionFactory.Create(SearchByCategory)
            ]);

    // Demo queries that require multi-step search
    var queries = new[]
    {
        "I need an outfit for a summer wedding — something elegant but not too formal",
        "What casual everyday tops do you have in cotton?",
        "I'm looking for something to wear to a party. Show me dresses and skirts."
    };

    var session = await agent.CreateSessionAsync();
    foreach (var query in queries)
    {
        Console.WriteLine($"\nCustomer: {query}");
        Console.WriteLine(new string('-', 60));
        var response = await agent.RunAsync(query, session);
        Console.WriteLine($"Consultant: {response.Text}");
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
    }
}
finally
{
    await postgresContainer.StopAsync();
    Console.WriteLine("PostgreSQL container stopped.");
}
