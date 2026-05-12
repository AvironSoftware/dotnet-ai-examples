using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using Pgvector;

namespace HybridSearchUsingPostgres;

public class VectorEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;

    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }

    public NpgsqlTsVector? SearchVector { get; set; }
}
