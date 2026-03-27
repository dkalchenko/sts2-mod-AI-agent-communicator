namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;

public class ClientResponseBase: GameActionClientResponseBase
{
    public required string RequestId { get; set; }
}