using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;
using Sts2ModAIAgentCommunicator.Core.Handlers;

namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator;

public class AgentPipeCommunicator: IAgentCommunicator, IAsyncDisposable
{
    private NamedPipeClientStream _clientPipe;
    private CancellationTokenSource _cancellationTokenSource = new();
    private ConcurrentQueue<IGameActionClientResponse> _clientMessagesQueue = new();
    
    public async ValueTask DisposeAsync()
    {
        await _clientPipe.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        Log.Info($"Initializing pipe.");
        _clientPipe = new NamedPipeClientStream(
            ".",
            "live-pipe",
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        
        Log.Info("Mocked pipe initialized.");
        
        // await ConnectPipeAsync();
        //
        // await SetupLoopsAsync();
    }

    private async Task ConnectPipeAsync()
    {
        if (_clientPipe is null)
        {
            throw new InvalidOperationException("Pipe not initialized.");
        }
        
        try
        {
            await _clientPipe.ConnectAsync(Configuration.ConnectionTimeoutInMilliseconds, _cancellationTokenSource.Token);
        }
        catch (TimeoutException)
        {
            Log.Warn($"Connection timed out. Retrying in {Configuration.WaitBetweenReconnectInMilliseconds}ms.");
            await Task.Delay(Configuration.WaitBetweenReconnectInMilliseconds, _cancellationTokenSource.Token);
            await _clientPipe.ConnectAsync(Configuration.ConnectionTimeoutInMilliseconds, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Log.Info($"Connection canceled.");
        }
    }

    private async Task SetupLoopsAsync()
    {
        var readTask = ReadLoop(_clientPipe, _cancellationTokenSource.Token);
        var writeTask = WriteLoop(_clientPipe, _cancellationTokenSource.Token);

        _ = Task.WhenAll(readTask, writeTask);
    }
    
    private async Task ReadLoop(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
            {
                await Task.Delay(Configuration.WaitTimeBetweenReadMessagesInMilliseconds, ct);
                continue;
            }
            
            await ReceiveMessageAsync(line, ct);
        }
    }

    private async Task WriteLoop(Stream stream, CancellationToken ct)
    {
        await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
        
        while (!ct.IsCancellationRequested)
        {
            var message = _clientMessagesQueue.TryDequeue(out var messageToSend) ? messageToSend : null;
            if (message is null)
            {
                await Task.Delay(Configuration.WaitTimeBetweenWriteMessagesInMilliseconds, ct);
                continue;
            }

            await writer.WriteLineAsync(message.ToJson());
        }
    }
    
    public Task SendMessageAsync(IGameActionClientResponse response)
    {
        Log.Info($"Sending message: {response.ToJson()}");
        _clientMessagesQueue.Enqueue(response);
        return Task.CompletedTask;
    }

    public async Task ReceiveMessageAsync(string message, CancellationToken cancellationToken)
    {
        var agentBaseRequest = JsonSerializer.Deserialize<AgentRequestBase>(message);

        if (agentBaseRequest is null)
        {
            Log.Error($"Received invalid message: {message}");
            return;
        }
        
        Log.Info($"Received message: {agentBaseRequest.Name}, Id: {agentBaseRequest.RequestId}");
        var handler = Agent2ClientHandlerFactory.GetHandler(agentBaseRequest);
        await handler.HandleAsync(message, cancellationToken);
    }
}
