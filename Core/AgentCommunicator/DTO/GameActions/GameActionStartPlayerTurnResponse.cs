namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.GameActions;

public class GameActionStartPlayerTurnResponse: GameActionClientResponseBase
{
    public ulong NetId { get; set; }
}