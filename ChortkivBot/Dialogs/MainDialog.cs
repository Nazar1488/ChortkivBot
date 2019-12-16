using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Travel.BlaBlaCar;
using ChortkivBot.Travel.Bus;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly UserState userState;

        public MainDialog(UserState userState, IRoutService routService, BusFinder busFinder, BlaBlaCarFinder blaBlaCarFinder)
            : base(nameof(MainDialog))
        {
            this.userState = userState; 

            AddDialog(new TravelDialog(busFinder, blaBlaCarFinder));
            AddDialog(new RoutDialog(routService));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Text = "Чим можу допомогти? \U0001F440",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "\U0001F68D Маршрутки", value: "/routes"),
                    new CardAction(ActionTypes.ImBack, "\U0001F698 Як доїхати?", value: "/travel"),
                    new CardAction(ActionTypes.ImBack, "\U0001F4F0 Новини", value: "/news"),
                    new CardAction(ActionTypes.ImBack, "\U0001F4E2 Події", value: "/events")
                }
            };

            var reply = MessageFactory.Attachment(card.ToAttachment());
            switch (stepContext.Context.Activity.Text)
            {
                case "/start":
                    card.Text =
                        $"Привіт \U0001F44B! Я бот міста Чорткова \U0001F60E.\n\nЧим можу допомогти? \U0001F440";
                    card.Images = new List<CardImage>
                    {
                        new CardImage
                        {
                            Url = "http://teren.in.ua/wp-content/uploads/2019/07/chortkiv1.jpg"
                        }
                    };
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                case "/routes":
                    return await stepContext.BeginDialogAsync(nameof(RoutDialog), null, cancellationToken);
                case "/travel":
                    return await stepContext.BeginDialogAsync(nameof(TravelDialog), null, cancellationToken);
                case "/news":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("У розрозбці... \U0001F51C"), cancellationToken);
                    break;
                case "/events":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("У розрозбці... \U0001F51C"), cancellationToken);
                    break;
            }

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
