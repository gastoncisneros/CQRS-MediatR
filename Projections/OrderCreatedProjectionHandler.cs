
public class OrderCreatedProjectionHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ReadDbContext _readDbContext;

    public OrderCreatedProjectionHandler(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(OrderCreatedEvent evt)
    {
        var order = new Order
        {
            Id = evt.OrderId,
            FirstName = evt.FirstName,
            LastName = evt.LastName,
            Statud = "Created",
            CreatedAt = DateTime.Now,
            TotalCost = evt.TotalCost
        };

        await _readDbContext.Orders.AddAsync(order);
        await _readDbContext.SaveChangesAsync();
    }
}