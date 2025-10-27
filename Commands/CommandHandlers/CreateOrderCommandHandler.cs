using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
{
    private readonly WriteDbContext _context;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IMediator _mediator;

    public CreateOrderCommandHandler(
        WriteDbContext context,
        IValidator<CreateOrderCommand> validator,
        IMediator mediator)
    {
        _context = context;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        Order newOrder = new Order()
        {
            FirstName = request.firstName,
            LastName = request.lastName,
            Statud = request.status,
            TotalCost = request.totalCost
        };

        await _context.Orders.AddAsync(newOrder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        OrderDTO orderDTO = new OrderDTO(
            newOrder.Id,
            newOrder.FirstName,
            newOrder.LastName,
            newOrder.Statud,
            newOrder.CreatedAt,
            newOrder.TotalCost);

        OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent(
            newOrder.Id,
            newOrder.FirstName,
            newOrder.LastName,
            newOrder.TotalCost
        );

        await _mediator.Publish(orderCreatedEvent);

        return orderDTO;
    }
}