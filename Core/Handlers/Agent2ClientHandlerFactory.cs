using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Agent;
using Sts2ModAIAgentCommunicator.Core.Handlers.Pile;

namespace Sts2ModAIAgentCommunicator.Core.Handlers;

public static class Agent2ClientHandlerFactory
{
    private static Dictionary<string, IAgent2ClientHandlerBase> Handlers { get; } = new();

    static Agent2ClientHandlerFactory()
    {
        Handlers.Add(nameof(GetPlayerHandPileRequest), new GetPlayerHandPileHandler());
    }
    
    public static IAgent2ClientHandlerBase GetHandler(IAgentRequest request)
    {
        return Handlers[request.Name];
    }
}