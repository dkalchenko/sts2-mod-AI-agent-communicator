using System.IO.Pipes;
using System.Text;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.Agent;

namespace Sts2ModAIAgentCommunicator.Core.Agent.Pipe;

public class PipeStream: IPipeStream
{
    private readonly NamedPipeClientStream _clientPipe;
    private readonly string _pipeName;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    
    public async ValueTask DisposeAsync()
    {
        await _clientPipe.DisposeAsync();
    }

    public PipeStream(string pipeName, string serverName = ".")
    {
        _pipeName = pipeName;
        _clientPipe = new NamedPipeClientStream(
            serverName,
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {

        if (_clientPipe is null)
        {
            throw new InvalidOperationException("Pipe not initialized.");
        }
        
        try
        {
            if (_clientPipe.IsConnected)
            {
                return;
            }
            
            await _clientPipe.ConnectAsync(Configuration.ConnectionTimeoutInMilliseconds, cancellationToken);
            _writer = new StreamWriter(_clientPipe, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
            _reader = new StreamReader(_clientPipe, Encoding.UTF8, leaveOpen: true);
        }
        catch (TimeoutException)
        {
            Log.Warn($"Connection timed out. Retrying in {Configuration.WaitBetweenReconnectInMilliseconds}ms.");
            await Task.Delay(Configuration.WaitBetweenReconnectInMilliseconds, cancellationToken);
            await _clientPipe.ConnectAsync(Configuration.ConnectionTimeoutInMilliseconds, cancellationToken);
            Log.Warn($"Connection timed out.");
        }
        catch (OperationCanceledException)
        {
            Log.Info($"Connection canceled.");
        }
    }

    public bool IsConnected() => true; // _clientPipe.IsConnected;


    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {

        if (!_clientPipe.IsConnected || _writer == null)
        {
            throw new InvalidOperationException($"Pipe {_pipeName} is not connected or writer is not initialized.");
        }

        await _writer.WriteLineAsync(message.AsMemory(), cancellationToken);
    }

    public async Task<string?> ReceiveMessageAsync(CancellationToken cancellationToken)
    {

        if (!_clientPipe.IsConnected || _reader == null)
        {
            throw new InvalidOperationException($"Pipe {_pipeName} is not connected or reader is not initialized.");
        }

        return await _reader.ReadLineAsync(cancellationToken);
    }
}
