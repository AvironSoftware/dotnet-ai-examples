using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VectorSearchUsingPostgres;

//NOTE: you must have Docker installed and running to run this sample

var openAiClient = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

//start container and get DbContext
var postgresContainer = await PostgresContainerFactory.GetPostgresContainerAsync();
var dbContext = postgresContainer.GetDbContext();

Console.WriteLine();
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Ask a question that searches our knowledge base!");
Console.ResetColor();
var text = Console.ReadLine();

var embeddings = await embeddingClient.GenerateAsync([text!]);

// Fetch closest match using cosine distance
var queryVector = new Vector(embeddings[0].Vector.ToArray());

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
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"*** Closest match: {nearest.Name} (similarity score: {allRecords.First().CosineDistance}) ***");
Console.ResetColor();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"Contents: {nearest.Contents}");
Console.ResetColor();

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("*** All matches and their similiarity scores (closer to 0 means more relevant): ***");
Console.ResetColor();
foreach (var record in allRecords.Skip(1))
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"{record.Record.Name} - {record.CosineDistance}");
    Console.ResetColor();
}
Console.WriteLine();

// Stop the container after use
await postgresContainer.StopAsync();