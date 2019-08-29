using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Bots
{
    public class Bot<T> : MainBot<T> where T : Dialog
    {
        public Bot(ConversationState conversationState, UserState userState, T dialog)
            : base(conversationState, userState, dialog)
        {
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var card = new HeroCard
                    {
                        Text = $"Привіт \U0001F44B, {member.Name}! Я бот міста Чорткова \U0001F60E. Чим можу допомогти? \U0001F440",
                        Buttons = new List<CardAction>
                        {
                            // Note that some channels require different values to be used in order to get buttons to display text.
                            // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                            // need to provide a value for other parameters like 'text' or 'displayText'.
                            new CardAction(ActionTypes.ImBack, "\U0001F68D Маршрутки", value: "/routes"),
                            new CardAction(ActionTypes.ImBack, "\U0001F4F0 Новини", value: "/news"),
                            new CardAction(ActionTypes.ImBack, "\U0001F4E2 Події", value: "/events")
                        },
                        Images = new List<CardImage>
                        {
                            new CardImage
                            {
                                Url = "http://teren.in.ua/wp-content/uploads/2019/07/chortkiv1.jpg"
                            }
                        }
                    };

                    var reply = MessageFactory.Attachment(card.ToAttachment());
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}