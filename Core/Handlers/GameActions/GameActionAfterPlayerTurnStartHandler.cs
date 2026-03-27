using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.GameActions;

namespace Sts2ModAIAgentCommunicator.Core.Handlers.GameActions;

public static class GameActionAfterPlayerTurnStartHandler
{
    public static void Handle(CombatState __, Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        
        var message = new GameActionStartPlayerTurnResponse
        {
            NetId = player.NetId
        };
        
        _ = SendData2Agent(message);
    }

    private static async Task SendData2Agent(IGameActionClientResponse response)
    {
        var communicator = AgentCommunicatorFactory.CreateCommunicator();
        
        await communicator.SendMessageAsync(response);
    }
}