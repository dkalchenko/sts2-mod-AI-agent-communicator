namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator;

public static class AgentCommunicatorFactory
{
    private static readonly Lazy<IAgentCommunicator> Singleton =
        new(() => new AgentPipeCommunicator(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static IAgentCommunicator CreateCommunicator()
    {
        return Singleton.Value;
    }
}
