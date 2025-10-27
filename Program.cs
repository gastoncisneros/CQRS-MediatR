using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WriteDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("WriteDbConnection"));
});

builder.Services.AddDbContext<ReadDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ReadDbConnection"));
});

builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapPost("/api/orders", async (IMediator mediator, Order order) =>
{
    try
    {
        CreateOrderCommand createOrderCommand = new CreateOrderCommand(order.FirstName, order.LastName, order.Statud, order.TotalCost);
        OrderDTO? newOrder = await mediator.Send(createOrderCommand);

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

app.MapGet("/api/orders/{id}", async (IMediator mediator, int id) =>
{
    GetOrderByIdQuery query = new GetOrderByIdQuery(id);
    OrderDTO? order = await mediator.Send(query);

    if (order != null) return Results.Ok(order);

    return Results.NotFound();
});

app.MapGet("/api/orders", async (IMediator mediator) =>
{
    var summaries = await mediator.Send(new GetOrdersSummaryQuery());

    return Results.Ok(summaries);
});


app.Run();
