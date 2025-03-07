using Pgvector;

namespace VectorSearchUsingPostgres;

public class VectorEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Contents { get; set; }
    public Vector Embedding { get; set; }
}