public interface ICommandHandler<TCommand, TResult> where TCommand : notnull
{
    public Task<TResult> HandleAsync(TCommand command);
}