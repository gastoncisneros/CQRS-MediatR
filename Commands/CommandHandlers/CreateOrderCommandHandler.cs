using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDTO>
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateOrderCommand> _validator;

    public CreateOrderCommandHandler(AppDbContext context, IValidator<CreateOrderCommand> validator)
    {
        _context = context;
        _validator = validator;
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

        return orderDTO;
    }
}