using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BaseConnection"));
});

var app = builder.Build();

app.MapPost("/api/orders", async (AppDbContext context, Order order) =>
{
    CreateOrderCommand createOrderCommand = new CreateOrderCommand(order.FirstName, order.LastName, order.Statud, order.TotalCost);
    Order? newOrder = await CreateOrderCommandHandler.Handle(createOrderCommand, context);

    if (newOrder is null)
    {
        return Results.BadRequest("Failed to create the order");
    }

    return Results.Created($"/api/orders/{newOrder.Id}", newOrder);
});

app.MapGet("/api/orders/{id}", async (AppDbContext context, int id) =>
{
    GetOrderByIdQuery query = new GetOrderByIdQuery(id);
    Order? order = await GetOrderByIdQueryHandler.Handle(query, context);

    if (order != null) return Results.Ok(order);

    return Results.NotFound();
});

app.Run();
