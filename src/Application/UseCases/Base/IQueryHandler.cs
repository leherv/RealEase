namespace Application.UseCases.Base;

public interface IQueryHandler<in TQuery, TQueryResult>
{
    Task<TQueryResult> Handle(TQuery mediaQuery, CancellationToken cancellationToken);
}