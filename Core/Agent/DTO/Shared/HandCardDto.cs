namespace Sts2ModAIAgentCommunicator.Core.Agent.DTO.Shared;

public class HandCardDto
{
    public string Type { get; set; }
    public int CurrentStarCost { get; set; }
    public int EnergyCost { get; set; }
    public uint CardNetId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}