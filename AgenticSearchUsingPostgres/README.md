# AgenticSearchUsingPostgres

Unlike the hybrid-search sample, this example gives the agent **multiple search tools** and lets it autonomously decide when, what, and how to search. The agent can:

- Search by natural language query (hybrid vector + text)
- Browse by category
- Get detailed product information
- Chain multiple searches to refine results

## Concepts

- **Agentic search**: The agent decides its own search strategy rather than following a fixed pipeline
- **Multi-tool orchestration**: Multiple search tools with different capabilities
- **Iterative refinement**: The agent can search multiple times, evaluating results and refining queries
- **Complex queries**: Demo queries require combining results from multiple searches (e.g., "outfit for a summer wedding")

## How It Differs from HybridSearchUsingPostgres

| | Hybrid search | Agentic search |
|---|---|---|
| Search trigger | Automatic before each LLM call | Agent decides when to search |
| Tools | Single search function | Multiple: search, browse by category, get details |
| Search strategy | Fixed | Agent-driven, iterative |
| Query complexity | Simple product lookups | Multi-step outfit recommendations |

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
dotnet run --project AgenticSearchUsingPostgres
```

The sample product data (`shared/data.sql`) is loaded automatically on first run.
