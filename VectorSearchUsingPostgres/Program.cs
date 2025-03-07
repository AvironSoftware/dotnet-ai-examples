using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VectorSearchUsingPostgres;

//NOTE: you must have Docker installed and running to run this sample

var openAiClient = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var embeddingClient = openAiClient.AsEmbeddingGenerator("text-embedding-3-small");

//start container and get DbContext
var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Ask a question that searches our knowledge base!");
var text = Console.ReadLine();

var embedding = await embeddingClient.GenerateEmbeddingAsync(text);

// Fetch closest match using cosine distance
var queryVector = new Vector(embedding.Vector);

// ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
var allRecords = await dbContext.Vectors
    .Select(v => new
    {
        Record = v,
        CosineDistance = v.Embedding.CosineDistance(queryVector)
    })
    .OrderBy(v => v.CosineDistance)
    .ToArrayAsync();

var nearest = allRecords.First().Record;

Console.WriteLine();
Console.WriteLine($"*** Closest match: {nearest.Name} (similarity score: {allRecords.First().CosineDistance}) ***");
Console.WriteLine($"Contents: {nearest.Contents}");

Console.WriteLine();
Console.WriteLine("*** All matches and their similiarity scores (closer to 0 means more relevant): ***");
foreach (var record in allRecords.Skip(1))
{
    Console.WriteLine($"{record.Record.Name} - {record.CosineDistance}");
}
Console.WriteLine();

// Stop the container after use
await postgresContainer.StopAsync();