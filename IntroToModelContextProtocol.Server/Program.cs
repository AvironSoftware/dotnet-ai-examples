using IntroToModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<RestaurantBookingPlugin>();

var app = builder.Build();

app.MapMcp();

app.Run();