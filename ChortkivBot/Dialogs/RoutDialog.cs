using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Dialogs.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Dialogs
{
    public class RoutDialog : ComponentDialog
    {
        private const string RoutInfo = "value-routInfo";
        private const string RoutChoice = "routChoice";
        private const string BusChoice = "busChoice";

        private readonly IRoutService routService;

        public RoutDialog(IRoutService routService) : base(nameof(RoutDialog))
        {
            this.routService = routService;

            AddDialog(new ChoicePrompt(RoutChoice) {Style = ListStyle.SuggestedAction});
            AddDialog(new ChoicePrompt(BusChoice) {Style = ListStyle.SuggestedAction});
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
            var choices = routes.Select(route => new Choice(route.Names[0])).ToList();
            stepContext.Values[RoutInfo] = new RoutDialogModel();
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Який маршрут тебе цікавить? \U0001F50D"),
                cancellationToken);
            return await stepContext.PromptAsync(RoutChoice, new PromptOptions
            {
                Choices = choices
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> StopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var routModel = (RoutDialogModel)stepContext.Values[RoutInfo];
            var result = (FoundChoice)stepContext.Result;
            var routId = await routService.GetRoutIdByName(result.Value);
            if (routId.HasValue)
            {
                routModel.RoutId = routId.Value;
                var stops = (await routService.GetRoutStops(routModel.RoutId)).ToList();
                var choices = stops.Select(stop => new Choice(stop.Names[0])).ToList();
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("З якої зупинки будеш їхати? \U0001F50D"),
                    cancellationToken);
                return await stepContext.PromptAsync(
                    BusChoice,
                    new PromptOptions
                    {
                        Choices = choices
                    }, 
                    cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Щось пішло не так \U0001F613. Спробуй ще раз \U0001F609"),
                cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> BusStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var routModel = (RoutDialogModel)stepContext.Values[RoutInfo];
            var result = (FoundChoice)stepContext.Result;
            var stopId = await routService.GetStopId(routModel.RoutId, result.Value);
            if (stopId.HasValue)
            {
                routModel.StopId = stopId.Value;
                var stops = (await routService.GetStopInfo(routModel.StopId)).ToList();
                var reply = stops.Any()
                    ? $"Твоя машрутка прибуде через {stops.First().Time} хв \U0001F60A"
                    : "Схоже, на даному маршруті зараз не курсує жоден автобус \U0001F614";

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(reply),
                    cancellationToken);

                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Щось пішло не так \U0001F613. Спробуй ще раз \U0001F609"),
                cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
        }
    }
}
