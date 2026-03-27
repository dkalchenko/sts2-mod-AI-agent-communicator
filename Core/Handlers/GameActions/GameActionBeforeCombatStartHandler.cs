using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Runs;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.GameActions;

namespace Sts2ModAIAgentCommunicator.Core.Handlers.GameActions;

public static class GameActionBeforeCombatStartHandler
{
    public static void Handle(IRunState __, CombatState? combatState)
    {
        ArgumentNullException.ThrowIfNull(combatState);

        var players = combatState.Players;
        
        var message = new GameActionCombatStartResponse
        {
            Player = CreateCombatCreature(players.First(LocalContext.IsMe).Creature),
            Allies = players.Where(x => !LocalContext.IsMe(x)).Select(x => x.Creature).Select(CreateCombatCreature),
            Enemies = combatState.Enemies.Select(CreateCombatCreature),
            CombatType = combatState.Encounter!.RoomType.ToString()
        };
        
        _ = SendData2Agent(message);
    }

    private static CombatCreature CreateCombatCreature(Creature creature)
    {
        return new CombatCreature
        {
            CreatureId = creature.ModelId.Entry,
            CreatureCombatId = creature.CombatId!.Value,
            CreatureName = creature.Name,
            CurrentHealth = creature.CurrentHp,
            MaxHealth = creature.MaxHp,
            Pets = creature.Pets.Select(CreateCombatCreature),
            NetId = creature.Player?.NetId
        };
    }

    private static async Task SendData2Agent(IGameActionClientResponse response)
    {
        var communicator = AgentCommunicatorFactory.CreateCommunicator();
        
        await communicator.SendMessageAsync(response);
    }
}