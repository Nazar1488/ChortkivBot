using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Core.Models;
using ChortkivBot.Dialogs.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Dialogs
{
    public class RoutDialog : ComponentDialog
    {
        private const string RoutInfo = "value-routInfo";

        private readonly IRoutService routService;

        public RoutDialog(IRoutService routService) : base(nameof(RoutDialog))
        {
            this.routService = routService;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RoutStepAsync,
                StopStepAsync,
                BusStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RoutStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var routes = (await routService.GetAvailableRoutes()).ToList();
            //var choices = routes.Select(route => new Choice(route.Names[0])).ToList();
            stepContext.Values[RoutInfo] = new RoutDialogModel();
            var actions = new List<CardAction>();
            foreach (var route in routes)
            {
                var action = new CardAction(ActionTypes.ImBack, route.Names[0], value: route.Id.ToString(), text: route.Id.ToString(), displayText: route.Id.ToString());
                actions.Add(action);
            }

            IMessageActivity reply;
            switch (stepContext.Context.Activity.ChannelId)
            {
                case Constants.FacebookClientName:
                    var card = new HeroCard
                    {
                        Text = "Який маршрут цікавить? \U0001F50D",
                        Buttons = actions
                    };

                    reply = MessageFactory.Attachment(card.ToAttachment());
                    break;
                default:
                    reply = MessageFactory.SuggestedActions(actions, "Який маршрут цікавить? \U0001F50D");
                    break;
            }

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions(), cancellationToken);
        }

        private async Task<DialogTurnResult> StopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var routModel = (RoutDialogModel)stepContext.Values[RoutInfo];
            var result = (string)stepContext.Result;
            var routId = await routService.GetRoutIdByName(result);
            if (routId.HasValue)
            {
                routModel.RoutId = routId.Value;
                var stops = (await routService.GetRoutStops(routModel.RoutId)).ToList();
                var actions = new List<CardAction>();
                foreach (var stop in stops)
                {
                    var action = new CardAction(ActionTypes.ImBack, stop.Names[0], value: stop.Id.ToString(), text: stop.Id.ToString(), displayText: stop.Id.ToString());
                    actions.Add(action);
                }

                IMessageActivity reply;
                switch (stepContext.Context.Activity.ChannelId)
                {
                    case Constants.FacebookClientName:
                        var card = new HeroCard
                        {
                            Text = "З якої зупинки будеш їхати? \U0001F50D",
                            Buttons = actions
                        };

                        reply = MessageFactory.Attachment(card.ToAttachment());
                        break;
                    default:
                        reply = MessageFactory.SuggestedActions(actions, "З якої зупинки будеш їхати? \U0001F50D");
                        break;
                }

                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions(), cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Щось пішло не так \U0001F613. Спробуй ще раз \U0001F609"),
                cancellationToken);
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> BusStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var routModel = (RoutDialogModel)stepContext.Values[RoutInfo];
            var result = (string)stepContext.Result;
            var stopId = await routService.GetStopId(routModel.RoutId, result);
            if (stopId.HasValue)
            {
                routModel.StopId = stopId.Value;
                var stops = (await routService.GetStopInfo(routModel.StopId)).ToList();
                var reply = stops.Any()
                    ? $"Твоя машрутка прибуде через {stops.First().Time} хв \U0001F60A"
                    : "Схоже, маршрутка прибуде не швидко \U0001F614";

                await stepContext.Context.SendActivityAsync(MessageFactory.SuggestedActions(new List<CardAction>(), reply),
                    cancellationToken);

                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Щось пішло не так \U0001F613. Спробуй ще раз \U0001F609"),
                cancellationToken);
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
        }
    }
}
