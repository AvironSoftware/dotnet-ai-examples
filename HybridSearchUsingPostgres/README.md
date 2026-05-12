# HybridSearchUsingPostgres

Demonstrates Retrieval-Augmented Generation (RAG) where the agent searches a women's clothing catalog using hybrid vector + full-text search before generating responses.

## Concepts

- **Hybrid search**: Combines vector similarity search (pgvector) with PostgreSQL full-text search
- **Reciprocal Rank Fusion (RRF)**: Merges rankings from both search methods into a single relevance score
- **Tool-based RAG**: The agent calls a search tool to find relevant products, then uses them as context
- **pgvector**: PostgreSQL extension for storing and querying vector embeddings

## Prerequisites

- Azure OpenAI endpoint and API key (needs both chat and embedding models)
- Docker (for pgvector container)
- .NET 10 SDK (C#) or Python 3.10+ (Python)

## Environment Variables

```bash
export POSTGRES_TALK_AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export POSTGRES_TALK_AZURE_OPENAI_API_KEY="your-api-key"
```

## Run

```bash
dotnet run --project HybridSearchUsingPostgres
```

The sample product data (`shared/data.sql`) is loaded automatically on first run.
