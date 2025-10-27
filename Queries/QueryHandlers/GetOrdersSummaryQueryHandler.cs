using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class GetOrdersSummaryQueryHandler : IRequestHandler<GetOrdersSummaryQuery, IEnumerable<OrderSummaryDTO>>
{
    private readonly ReadDbContext _context;

    public GetOrdersSummaryQueryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderSummaryDTO>> Handle(GetOrdersSummaryQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<OrderSummaryDTO> orders = await _context.Orders
        .AsNoTracking()
        .Select(o => new OrderSummaryDTO(
            o.Id,
            string.Format("{0} {1}", o.FirstName, o.LastName),
            o.Statud,
            o.TotalCost
        ))
        .ToListAsync(cancellationToken);

        return orders;
    }
}