using System.Text.Json;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Agent;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Pile;
using Sts2ModAIAgentCommunicator.Core.AgentCommunicator.DTO.Shared;

namespace Sts2ModAIAgentCommunicator.Core.Handlers.Pile;

public class GetPlayerHandPileHandler: IAgent2ClientHandlerBase
{
    public async Task HandleAsync(string message, CancellationToken cancellationToken)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            Log.Error($"Received GetPlayerHandPileRequest while not in combat. Message: {message}");
            return;
        }

        var combatState = CombatManager.Instance.DebugOnlyGetState();
        
        if (combatState is null)
        {
            Log.Error($"Combat state is null. Message: {message}");
            return;
        }
        
        var requestMessage = JsonSerializer.Deserialize<GetPlayerHandPileRequest>(message);

        if (requestMessage is null) 
        {
            Log.Error($"Request message cannot be parsed to {nameof(GetPlayerHandPileRequest)}. Message: {message}");
            return;
        }

        var player = combatState.GetPlayer(requestMessage.PlayerNetId);

        if (player is null) 
        {
            Log.Error($"Player was not found. Message: {message}");
            return;
        }

        var handPile = PileType.Hand.GetPile(player).Cards;
        var netCombatCardDb = NetCombatCardDb.Instance;

        var response = new GetPlayerHandPileResponse
        {
            RequestId = requestMessage.RequestId,
            PlayerNetId = player.NetId,
            Cards = handPile.Select(x => new HandCardDto
            {
                CurrentStarCost = x.CurrentStarCost,
                EnergyCost = x.EnergyCost.GetResolved(),
                Type = x.Type.ToString(),
                Description = x.Description.GetFormattedText(),
                Name = x.Title,
                CardNetId = netCombatCardDb.GetCardId(x)
            })
        };
        
        var communicator = AgentCommunicatorFactory.CreateCommunicator();

        await communicator.SendMessageAsync(response);
    }
}