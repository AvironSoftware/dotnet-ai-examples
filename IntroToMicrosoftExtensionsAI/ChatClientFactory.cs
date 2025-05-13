using Microsoft.Extensions.AI;
using OpenAI;

namespace IntroToMicrosoftExtensionsAI;

public static class ChatClientFactory
{
    public static IChatClient CreateChatClient()
    {
        var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");  //add your OpenAI API key here
        var model = "gpt-4o";

        //where the OpenAI client gets built
        var client = new OpenAIClient(openAIApiKey);

        //where we get our chat client
        IChatClient chatClient = client
            .GetChatClient(model)
            .AsIChatClient()    //NOTE: there is a GetChatClient but that is not the same!
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
        
        return chatClient;
    }
}