using OpenAI;
using OpenAI.Chat;

namespace IntroToAgentFramework;

public static class ChatClientFactory
{
    public static ChatClient CreateChatClient()
    {
        var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");  //add your OpenAI API key here
        var model = "gpt-4o";

        //where the OpenAI client gets built
        var client = new OpenAIClient(openAIApiKey);

        //where we get our chat client
        return client.GetChatClient(model);
    }
}