namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;

public class AgentRequestBase: IAgentRequest
{
    public string Name { get; set; }
    public string RequestId { get; set; }
}