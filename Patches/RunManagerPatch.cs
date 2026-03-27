using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using Sts2ModAIAgentCommunicator.Core;

namespace Sts2ModAIAgentCommunicator.Patches;

[HarmonyPatch(typeof(RunManager))]
public class RunManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(RunManager.Launch))]
    static void PrefixRunLaunch()
    {
        GameEventsHub.SubscribeCombatManagerEvents();
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RunManager.OnEnded))]
    static void PostfixOnEnded()
    {
        GameEventsHub.UnsubscribeCombatManagerEvents();
    }
}