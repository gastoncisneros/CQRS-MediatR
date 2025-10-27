using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO?>
{
    private readonly ReadDbContext _context;

    public GetOrderByIdQueryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDTO?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        Order? order = await _context.Orders
        .AsNoTracking()
        .FirstOrDefaultAsync(o => o.Id == request.orderId);

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