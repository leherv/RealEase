namespace Application.UseCases.Base.CQS;

// interface with 1 impl right now - PipelineBehaviours can be added later
public interface ICommandDispatcher
{
    Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken = default);
}