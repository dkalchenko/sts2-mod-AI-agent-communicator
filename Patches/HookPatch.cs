using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Runs;
using Sts2ModAIAgentCommunicator.Core.Handlers.GameActions;

namespace Sts2ModAIAgentCommunicator.Patches;

[HarmonyPatch(typeof(Hook))]
public class HookPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hook.BeforeCombatStart))]
    public static void PostfixBeforeCombatStart(IRunState runState, CombatState? combatState)
    {
        GameActionBeforeCombatStartHandler.Handle(runState, combatState);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hook.AfterPlayerTurnStart))]
    public static void PostfixAfterPlayerTurnStart(CombatState combatState, Player player)
    {

        GameActionAfterPlayerTurnStartHandler.Handle(combatState, player);
    }
}
