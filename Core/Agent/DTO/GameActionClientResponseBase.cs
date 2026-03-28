using System.Text.Json;

namespace Sts2ModAIAgentCommunicator.Core.Agent.DTO;

public class GameActionClientResponseBase: IClientResponse
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, GetType());
    }
}