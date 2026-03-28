using System.Collections.Concurrent;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.Agent.Pipe;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO;
using Sts2ModAIAgentCommunicator.Core.Handlers;
using Sts2ModAIAgentCommunicator.Core.Settings;

namespace Sts2ModAIAgentCommunicator.Core.Agent;

public class AgentCommunicator: IAsyncDisposable
{
    private class EnqueuedMessage
    {
        public required string Message { get; init; }
        public int RetryCount { get; set; }
    } 
    
    private static readonly Lazy<AgentCommunicator> _instance = new(() => new AgentCommunicator(new AgentCommunicatorSettings()));
    public static AgentCommunicator Instance => _instance.Value;

    private readonly IPipeStream _stream;
    private AgentCommunicatorSettings _settings;
    private readonly ConcurrentQueue<EnqueuedMessage> _requestsToSend = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private AgentCommunicator(AgentCommunicatorSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
        _stream = new PipeStream(settings.AgentName, settings.Host);   
    }
    
    public async Task InitializeAsync()
    {
        await _stream.ConnectAsync(_cancellationTokenSource.Token);
        _ = Task.WhenAll(
            SendMessagesLoopAsync(_cancellationTokenSource.Token),
            ReceiveMessagesLoopAsync(_cancellationTokenSource.Token),
            ReconnectLoopAsync(_cancellationTokenSource.Token)
        );
    }

    public void SendMessage(IClientResponse message)
    {
        _requestsToSend.Enqueue(new EnqueuedMessage
        {
            Message = message.ToJson(),
            RetryCount = 0
        });
    }

    private async Task ReconnectLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _stream.ConnectAsync(cancellationToken);
            await Task.Delay(Configuration.WaitBetweenReconnectInMilliseconds, cancellationToken);
        }
    }

    private async Task SendMessagesLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await HandleEnqueuedMessageAsync(cancellationToken);
        }
    }

    private async Task ReceiveMessagesLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ReceiveMessageAsync(cancellationToken);
        }
    }
    
    private async Task HandleEnqueuedMessageAsync(CancellationToken cancellationToken)
    {
        var message = _requestsToSend.TryDequeue(out var result) ? result : null;

        if (message is null)
        {
            await Task.Delay(Configuration.WaitTimeBetweenWriteMessagesInMilliseconds, cancellationToken);
            return;
        }
        
        if (!_stream.IsConnected())
        {
            Log.Error("We have message to send but stream is closed. Message: " + message.Message);
            _requestsToSend.Enqueue(message);
            return;
        }
        
        try
        { 
            await _stream.SendMessageAsync(message.Message, cancellationToken);
            await Task.Delay(Configuration.WaitTimeBetweenWriteMessagesInMilliseconds, cancellationToken);
        }
        catch (Exception e)
        {
            Log.Error("Cannot send message to server. Message: " + message.Message + ". Error: " + e.Message);

            if (message.RetryCount > Configuration.MaxSendRetryCount)
            {
                Log.Warn("Max retry count reached. Message: " + message.Message);
                return;
            }

            message.RetryCount += 1;
            
            _requestsToSend.Enqueue(message);

            await Task.Delay(Configuration.WaitTimeBetweenWriteMessagesInMilliseconds, cancellationToken);
        }
    }

    private async Task ReceiveMessageAsync(CancellationToken cancellationToken)
    {
        var message = await _stream.ReceiveMessageAsync(cancellationToken);

        if (message is null)
        {
            await Task.Delay(Configuration.WaitTimeBetweenReadMessagesInMilliseconds, cancellationToken);
            return;
        }
        
        await Agent2ClientHandlerHub.HandleAsync(message, cancellationToken);
        await Task.Delay(Configuration.WaitTimeBetweenReadMessagesInMilliseconds, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        await _stream.DisposeAsync();
    }
}