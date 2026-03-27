using MegaCrit.Sts2.Core.Logging;

namespace Sts2ModAIAgentCommunicator.Core;

public class GameEventsHub
{
    public static void SubscribeCombatManagerEvents()
    {
        Log.Info("Subscribing to combat manager events");
    }
    
    public static void UnsubscribeCombatManagerEvents()
    {
    }
}