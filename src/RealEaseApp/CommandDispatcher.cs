using Application.UseCases.Base;

namespace RealEaseApp;

internal class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation = default)
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
        return handler.Handle(command, cancellation);
    }
}