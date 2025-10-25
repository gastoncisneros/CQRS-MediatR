using Microsoft.EntityFrameworkCore;

public class CreateOrderCommandHandler
{
    public static async Task<Order> Handle(CreateOrderCommand command, AppDbContext context)
    {
        Order newOrder = new Order()
        {
            FirstName = command.firstName,
            LastName = command.lastName,
            Statud = command.status,
            TotalCost = command.totalCost
        };

        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();

        return newOrder;
    }
}