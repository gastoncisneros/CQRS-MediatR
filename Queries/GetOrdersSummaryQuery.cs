using MediatR;

public record GetOrdersSummaryQuery() : IRequest<List<OrderSummaryDTO>>;