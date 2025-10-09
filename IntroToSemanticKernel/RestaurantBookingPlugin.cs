using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace IntroToSemanticKernel;

public class RestaurantBookingPlugin
{
    private readonly RestaurantDbContext _dbContext;

    public RestaurantBookingPlugin(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [KernelFunction("get_restaurants_available_to_book")]
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
    
    [KernelFunction("book_restaurant")]
    [Description("""
                 Used to book a restaurant for a specific date/time.
                 """)]
    public string BookRestaurant(
        [Description("The name of the restaurant to book.")]
        string restaurantName, 
        
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
    
    [KernelFunction("get_booked_restaurants")]
    public RestaurantBooking[] GetBookedRestaurants()
    {
        return _dbContext.RestaurantBookings.ToArray();
    }
}