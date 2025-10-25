using Microsoft.EntityFrameworkCore;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDTO>
{
    private readonly AppDbContext _context;

    public CreateOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDTO> HandleAsync(CreateOrderCommand command)
    {
        Order newOrder = new Order()
        {
            FirstName = command.firstName,
            LastName = command.lastName,
            Statud = command.status,
            TotalCost = command.totalCost
        };

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();

        OrderDTO orderDTO = new OrderDTO(
            newOrder.Id,
            newOrder.FirstName,
            newOrder.LastName,
            newOrder.Statud,
            newOrder.CreatedAt,
            newOrder.TotalCost);

        return orderDTO;
    }
}