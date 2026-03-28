using System.Text.Json;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using Sts2ModAIAgentCommunicator.Core.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Agent;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Pile;
using Sts2ModAIAgentCommunicator.Core.Agent.DTO.Shared;

namespace Sts2ModAIAgentCommunicator.Core.Handlers.Pile;

public class InputPlayHandCardHandler: IAgent2ClientHandler
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
        
        var requestMessage = JsonSerializer.Deserialize<InputPlayHandCardRequest>(message);

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

        if (!NetCombatCardDb.Instance.TryGetCard(requestMessage.CardNetId, out var card))
        {
            Log.Error($"Card was not found. Message: {message}");
            return;
        }

        if (!card.CanPlay())
        {
            Log.Error($"Cannot play card. Message: {message}");
            return;
        }
        
        Creature? target = null;
        
        if (requestMessage.CombatCreatureId is null)
        {
            target = combatState.GetCreature(requestMessage.CombatCreatureId);
            if (target is null)
            {
                Log.Error($"Cannot find creature. Message: {message}");
                return;
            }
        }
        
        if (!card.TryManualPlay(target))
        {
            Log.Error($"Cannot play this card. Message: {message}");
            return;
        }
        
        AgentCommunicator.Instance.SendMessage(new AckResponse{ RequestId = requestMessage.RequestId });
    }
}