using System.Text.Json;

namespace Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO;

public class GameActionClientResponseBase: IGameActionClientResponse
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, GetType());
    }
}