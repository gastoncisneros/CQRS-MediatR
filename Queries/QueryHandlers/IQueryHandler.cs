public interface IQueryHandler<TQuery, TResult>
{
    public Task<TResult?> HandleAsync(TQuery query);
}