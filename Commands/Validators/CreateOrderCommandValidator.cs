using FluentValidation;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(o => o.firstName).NotEmpty();
        RuleFor(o => o.lastName).NotEmpty();
        RuleFor(o => o.status).NotEmpty();
        RuleFor(o => o.totalCost).GreaterThan(0);
    }
}