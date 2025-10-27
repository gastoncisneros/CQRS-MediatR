using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDTO>
{
    private readonly WriteDbContext _context;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(
        WriteDbContext context,
        IValidator<CreateOrderCommand> validator,
        IEventPublisher eventPublisher)
    {
        _context = context;
        _validator = validator;
        _eventPublisher = eventPublisher;
    }

    public async Task<OrderDTO> HandleAsync(CreateOrderCommand command)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

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

        OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent(
            newOrder.Id,
            newOrder.FirstName,
            newOrder.LastName,
            newOrder.TotalCost
        );

        await _eventPublisher.PublishAsync(orderCreatedEvent);

        return orderDTO;
    }
}