using Microsoft.EntityFrameworkCore;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDTO>
{
    private readonly AppDbContext _context;

    public GetOrderByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDTO?> HandleAsync(GetOrderByIdQuery query)
    {
        Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == query.orderId);

        if (order is null) return null;

        OrderDTO orderDTO = new OrderDTO(
            order.Id,
            order.FirstName,
            order.LastName,
            order.Statud,
            order.CreatedAt,
            order.TotalCost);

        return orderDTO;
    }
}