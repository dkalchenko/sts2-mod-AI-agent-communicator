namespace Sts2ModAIAgentCommunicator.Core.Handlers;

public interface IAgent2ClientHandlerBase
{
    public abstract Task HandleAsync(string message, CancellationToken cancellationToken);
}