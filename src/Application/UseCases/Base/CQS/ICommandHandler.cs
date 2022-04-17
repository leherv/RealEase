namespace Application.UseCases.Base.CQS;

public interface ICommandHandler<in TCommand, TCommandResult>
{
    Task<TCommandResult> Handle(TCommand subscribeMedia, CancellationToken cancellationToken);
}