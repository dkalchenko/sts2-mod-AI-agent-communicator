namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.GameActions;

public class GameActionCombatStartResponse: GameActionClientResponseBase
{
    public required CombatCreature Player { get; set; }
    public required IEnumerable<CombatCreature> Allies { get; set; }
    public required IEnumerable<CombatCreature> Enemies { get; set; }
    public required string CombatType { get; set; }
}

public class CombatCreature
{
    public ulong? NetId { get; set; }
    public uint CreatureCombatId { get; set; }
    public string CreatureId { get; set; }
    public string CreatureName { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public IEnumerable<CombatCreature> Pets { get; set; }
}