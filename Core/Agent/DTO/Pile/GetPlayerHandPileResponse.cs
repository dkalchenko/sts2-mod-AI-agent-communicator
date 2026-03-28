using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Shared;

namespace Sts2ModAIAgentCommunicator.Core.Agent.DTO.Pile;

public class GetPlayerHandPileResponse : ClientResponseBase
{
    public IEnumerable<HandCardDto> Cards { get; set; } 
    public ulong PlayerNetId { get; set; }
}