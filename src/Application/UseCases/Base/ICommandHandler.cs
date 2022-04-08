namespace Application.UseCases.Base;

public interface ICommandHandler<in TCommand, TCommandResult>
{
    Task<TCommandResult> Handle(TCommand subscribeMedia, CancellationToken cancellationToken);
}