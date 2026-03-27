using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Agent;

namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator;

public interface IAgentCommunicator
{
    public Task InitializeAsync();
    public Task SendMessageAsync(IGameActionClientResponse response);
}