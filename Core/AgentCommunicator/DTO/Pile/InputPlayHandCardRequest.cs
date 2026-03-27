namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Pile;

public class InputPlayHandCardRequest: AgentRequestBase
{
    public uint CardNetId { get; set; }
    public uint PlayerNetId { get; set; }
    public uint? CombatCreatureId { get; set; }
}