# Dotnet AI Examples

Welcome to **Dotnet AI Examples** – a collection of simple, focused sample projects that demonstrate how to work with LLMs (Large Language Models) using .NET.  
These examples help .NET developers get hands-on experience with **conversation loops**, **tool calling**, **document ingestion**, and **vector search** using libraries like **OpenAI**, **Azure OpenAI**, **Microsoft Semantic Kernel**, **Microsoft.Extensions.AI**, and **Pgvector**.

Whether you're calling .NET methods with natural language, asking questions over ingested files, or doing similarity search over embeddings – this repo has a starting point for you.

---

## 📦 Sample Projects

| Sample                                                                    | Description                                                                                                                                      | Tech Used                          |
| ------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------- |
| [OpenAITest](./OpenAITest/Program.cs)                                     | Illustrates how to interact with OpenAI in a conversational loop.                                                                                | `OpenAI.Chat`                      |
| [AzureOpenAITest](./AzureOpenAITest/Program.cs)                           | Illustrates how to interact with Azure OpenAI in a conversational loop.                                                                          | `OpenAI.Chat`<br>`Azure.AI.OpenAI` |
| [IntroToKernelMemory](./IntroToKernelMemory/Program.cs)                   | Demonstrates how to ingest text files into Kernel Memory and use an LLM to answer natural language questions based on their content.             | `Microsoft.KernelMemory`           |
| [IntroToMicrosoftExtensionsAI](./IntroToMicrosoftExtensionsAI/Program.cs) | Demonstrates how to use AIFunctionFactory to enable tool calling, allowing natural language input to trigger the C# method.                      | `Microsoft.Extensions.AI`          |
| [IntroToSemanticKernel](./IntroToSemanticKernel/Program.cs)               | Shows tool calling using KernelFunction and KernelPlugin to expose C# methods as AI-invokable functions.                                         | `Microsoft.SemanticKernel`         |
| [VectorSearchUsingPostgres](./VectorSearchUsingPostgres/Program.cs)       | Illustrates how to generate embeddings with OpenAI and perform semantic search against a Postgres database using pgvector for vector similarity. | `Pgvector`                         |

---

## ⚙️ Setup

1. **Clone the repo:**

   ```bash
   git clone https://github.com/AvironSoftware/dotnet-ai-examples.git
   cd dotnet-ai-examples
   ```

2. **Set your OpenAI API key:**

   - **Option 1:**  
     Instead of loading the key from an environment variable like this:

     ```csharp
     Environment.GetEnvironmentVariable("OPENAI_API_KEY")
     ```

     You can hardcode your key directly for quick testing:

     ```csharp
     "{your_openai_api_key}"
     ```

     > Just replace `{your_openai_api_key}` with your actual OpenAI key (in quotes).  
     > **Note:** Avoid committing this version to source control for security reasons.

   - **Option 2:**  
     Set it as an environment variable:

     ```bash
     export OPENAI_API_KEY=your-key-here
     ```

   - On Windows (PowerShell):
     ```powershell
     $env:OPENAI_API_KEY="your-key-here"
     ```

3. **Run Docker Desktop**
   - **IntroToSemanticKernel** and **VectorSearchUsingPostgres** relies on Docker Desktop to be running for [PostgressContainerFactory](./VectorSearchUsingPostgres/PostgresContainerFactory.cs) to initialize.

---

## 🔑 How to Get an OpenAI API Key

1. Go to [https://platform.openai.com/account/api-keys](https://platform.openai.com/account/api-keys)
2. Sign in or create an OpenAI account.
3. Click **"Create new secret key"**.
4. Copy the key and keep it somewhere safe — you won’t be able to see it again!

---

Enjoy exploring AI in .NET!
