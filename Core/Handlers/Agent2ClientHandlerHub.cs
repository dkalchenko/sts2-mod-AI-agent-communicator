using System.Text.Json;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Pile;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Shared;
using Sts2ModAIAgentCommunicator.Core.Handlers.Pile;

namespace Sts2ModAIAgentCommunicator.Core.Handlers;

public static class Agent2ClientHandlerHub
{
    private static Dictionary<string, IAgent2ClientHandler> Handlers { get; } = new();

    static Agent2ClientHandlerHub()
    {
        Handlers.Add(nameof(GetPlayerHandPileRequest), new GetPlayerHandPileHandler());
        Handlers.Add(nameof(InputPlayHandCardRequest), new InputPlayHandCardHandler());
    }
    
    public static async Task HandleAsync(string message, CancellationToken cancellationToken)
    {
        using var requestMessage = JsonDocument.Parse(message);;

        var name = requestMessage.RootElement.GetProperty("Name").GetString();
        var requestId = requestMessage.RootElement.GetProperty("RequestId").GetString();
        
        if (name is null || requestId is null)
        {
            var errorMessage = $"Cannot get request Id or name from request from {message}";
            Log.Error(errorMessage);
            AgentCommunicator.Instance.SendMessage(new ErrorResponse{ Message = errorMessage, RequestId = "" });
            return;
        }

        try
        {
            var handler = Handlers[name];
            await handler.HandleAsync(message, cancellationToken);
        }
        catch (Exception e)
        {
            Log.Error($"Error handling message: {message}. Error: {e.Message}");
            
            AgentCommunicator.Instance.SendMessage(new ErrorResponse{ Message = e.Message, RequestId = requestId});
        }
    }
}