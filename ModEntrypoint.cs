using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Sts2ModAIAgentCommunicator.Core.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent;

namespace Sts2ModAIAgentCommunicator;

[ModInitializer("Initialize")]
public class ModEntrypoint
{
    public static void Initialize()
    {
        var harmony = new Harmony("AIInterface.patch");
        harmony.PatchAll();
        _ = PipelineInitializeAsync().ContinueWith(ResultHandler, TaskScheduler.Default);
    }
    
    private static async Task PipelineInitializeAsync()
    {
        await AgentCommunicator.Instance.InitializeAsync();
    }
    
    private static void ResultHandler(Task task)
    {
        if (task.Exception != null)
        {
            Log.Error($"Communicator initialization failed. {task.Exception.Message}");
        }
    }
}