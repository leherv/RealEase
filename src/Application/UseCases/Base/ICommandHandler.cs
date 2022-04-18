namespace Application.UseCases.Base;

public interface ICommandHandler<in TCommand, TCommandResult>
{
    Task<TCommandResult> Handle(TCommand scrapeNewReleasesCommand, CancellationToken cancellationToken);
}