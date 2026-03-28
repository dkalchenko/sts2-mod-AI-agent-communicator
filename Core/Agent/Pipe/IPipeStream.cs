namespace Sts2ModAIAgentCommunicator.Core.Agent.Pipe;

public interface IPipeStream: IAsyncDisposable
{
    public Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
    public Task ConnectAsync(CancellationToken cancellationToken);
    public bool IsConnected();
    public Task<string?> ReceiveMessageAsync(CancellationToken cancellationToken);
}