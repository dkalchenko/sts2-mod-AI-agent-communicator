using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.GameActions;

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
        
        AgentCommunicator.Instance.SendMessage(message);
    }
}