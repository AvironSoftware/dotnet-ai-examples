using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace IntroToAgentFramework;

public class RestaurantBookingService
{
    private readonly RestaurantDbContext _dbContext;

    public RestaurantBookingService(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [Description("Gets a list of restaurants available to book.")]
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
    
    [Description("Used to book a restaurant for a specific date/time.")]
    public string BookRestaurant(
        [Description("The name of the restaurant to book.")]
        string restaurantName, 
        [Description("The date and time of the reservation in UTC.")]
        DateTime reservationDateTimeUtc)
    {
        var booking = new RestaurantBooking
        {
            RestaurantName = restaurantName,
            ReservationDateTimeUtc = DateTime.SpecifyKind(reservationDateTimeUtc, DateTimeKind.Utc)
        };
        _dbContext.RestaurantBookings.Add(booking);
        _dbContext.SaveChanges();
        
        return $"Booked {restaurantName} for {reservationDateTimeUtc}";
    }
    
    [Description("Gets a list of all booked restaurants.")]
    public RestaurantBooking[] GetBookedRestaurants()
    {
        return _dbContext.RestaurantBookings.ToArray();
    }

    public AITool[] AsAITools()
    {
        return
        [
            AIFunctionFactory.Create(GetBookedRestaurants, name: "get_booked_restaurants"),
            AIFunctionFactory.Create(GetRestaurantsAvailableToBook, name: "get_restaurants_available_to_book"),
            AIFunctionFactory.Create(BookRestaurant, name: "book_restaurant")
        ];
    }
}