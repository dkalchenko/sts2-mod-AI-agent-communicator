namespace Sts2ModAIAgentCommunicator.Core.Agent.DTO.GameActions;

public class GameActionStartPlayerTurnResponse: GameActionClientResponseBase
{
    public ulong NetId { get; set; }
}