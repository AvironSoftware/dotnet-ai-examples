using Microsoft.SemanticKernel;

namespace IntroToSemanticKernel;

public class RestaurantBookingPlugin
{
    [KernelFunction]
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