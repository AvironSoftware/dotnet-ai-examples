using ModelContextProtocol.Server;

namespace IntroToModelContextProtocol.Server;

public class RestaurantBookingPlugin
{
    [McpServerTool]
    public string[] GetRestaurantsAvailableToBook()
    {
        return
        [
            "McDonald's",
            "Five Guys",
            "Chili's",
            "Ruth's Chris"
        ];
    }
}