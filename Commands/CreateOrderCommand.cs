using MediatR;

public record CreateOrderCommand(
    string firstName,
    string lastName,
    string status,
    decimal totalCost
) : IRequest<OrderDTO>;