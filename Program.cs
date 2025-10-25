using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BaseConnection"));
});

var app = builder.Build();

app.MapPost("/api/orders", async (AppDbContext context, Order order) =>
{
    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.MapGet("/api/orders/{id}", async (AppDbContext context, int id) =>
{
    Order? order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);

    if (order != null) return Results.Ok(order);

    return Results.NotFound();
});

app.Run();
