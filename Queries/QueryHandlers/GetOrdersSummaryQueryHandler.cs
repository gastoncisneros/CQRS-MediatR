using Microsoft.EntityFrameworkCore;
using System.Linq;

public class GetOrdersSummaryQueryHandler : IQueryHandler<GetOrdersSummaryQuery, IEnumerable<OrderSummaryDTO>>
{
    private readonly AppDbContext _context;

    public GetOrdersSummaryQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderSummaryDTO>?> HandleAsync(GetOrdersSummaryQuery query)
    {
        IEnumerable<OrderSummaryDTO> orders = await _context.Orders
        .Select(o => new OrderSummaryDTO(
            o.Id,
            string.Format("{0} {1}", o.FirstName, o.LastName),
            o.Statud,
            o.TotalCost
        ))
        .ToListAsync();

        return orders;
    }
}