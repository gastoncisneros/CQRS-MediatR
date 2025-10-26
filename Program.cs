using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BaseConnection"));
});

builder.Services.AddDbContext<WriteDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("WriteDbConnection"));
});

builder.Services.AddDbContext<ReadDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ReadDbConnection"));
});

builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, OrderDTO>, CreateOrderCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDTO>, GetOrderByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrdersSummaryQuery, IEnumerable<OrderSummaryDTO>>, GetOrdersSummaryQueryHandler>();

builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();

builder.Services.AddScoped<IEventPublisher, ConsoleEventPublisher>();

var app = builder.Build();

app.MapPost("/api/orders", async (ICommandHandler<CreateOrderCommand, OrderDTO> commandHandler, Order order) =>
{
    try
    {
        CreateOrderCommand createOrderCommand = new CreateOrderCommand(order.FirstName, order.LastName, order.Statud, order.TotalCost);
        OrderDTO? newOrder = await commandHandler.HandleAsync(createOrderCommand);

        if (newOrder is null)
        {
            return Results.BadRequest("Failed to create the order");
        }

        return Results.Created($"/api/orders/{newOrder.Id}", newOrder);
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors.Select(x => new { x.PropertyName, x.ErrorMessage });

        return Results.BadRequest(errors);
    }
});

app.MapGet("/api/orders/{id}", async (IQueryHandler<GetOrderByIdQuery, OrderDTO> queryHandler, int id) =>
{
    GetOrderByIdQuery query = new GetOrderByIdQuery(id);
    OrderDTO? order = await queryHandler.HandleAsync(query);

    if (order != null) return Results.Ok(order);

    return Results.NotFound();
});

app.MapGet("/api/orders", async (IQueryHandler<GetOrdersSummaryQuery, IEnumerable<OrderSummaryDTO>> queryHandler) =>
{
    var summaries = await queryHandler.HandleAsync(new GetOrdersSummaryQuery());

    return Results.Ok(summaries);
});

app.Run();
