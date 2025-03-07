using Microsoft.Extensions.AI;
using Moq;
using Xunit.Abstractions;

namespace IntroToMicrosoftExtensionsAI.Tests;

public class ToolCallTest
{
    private readonly ITestOutputHelper _outputHelper;

    public ToolCallTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    [Theory]
    [InlineData("What restaurants are available to book?")]
    [InlineData("What restaurants can I book a table at?")]
    public async Task Tool_call_is_invoked_successfully(string message)
    {
        //arrange
        var chatClient = ChatClientFactory.CreateChatClient();
        var chatHistory = new List<ChatMessage>
        {
            new (ChatRole.System, "You are a helpful restaurant reservation booking assistant.")
        };
        var restaurantPlugin = new Mock<IRestaurantPlugin>();
        restaurantPlugin.Setup(x => x.GetRestaurantsAvailableToBook()).Returns("Just Bob Evans");
        
        var chatOptions = new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(restaurantPlugin.Object.GetRestaurantsAvailableToBook)
            ],
        };
        
        //act
        var response = await chatClient.GetResponseAsync(message, chatOptions);
        _outputHelper.WriteLine(response.Message.Text);

        //assert
        restaurantPlugin.Verify(x => x.GetRestaurantsAvailableToBook(), Times.Once);
        
        //do you want to take it this far...?
        Assert.Contains("Bob Evans", response.Message.Text);
    }
}

public interface IRestaurantPlugin
{
    string GetRestaurantsAvailableToBook();
}