namespace Sts2ModAIAgentCommunicator.Core.Agent.DTO;

public class AgentRequestBase: IAgentRequest
{
    public string Name { get; set; }
    public string RequestId { get; set; }
}