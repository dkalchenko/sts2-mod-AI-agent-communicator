namespace Sts2ModAIAgentCommunicator.Core.Handlers;

public interface IAgent2ClientHandler
{
    public abstract Task HandleAsync(string message, CancellationToken cancellationToken);
}