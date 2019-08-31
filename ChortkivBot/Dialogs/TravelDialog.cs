using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChortkivBot.Dialogs.Models;
using ChortkivBot.Travel.BlaBlaCar;
using ChortkivBot.Travel.Bus;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Dialogs
{
    public class TravelDialog : ComponentDialog
    {
        private const string TravelInfo = "value-travelInfo";
        private const string From = "Чортків";

        private readonly BusFinder busFinder;
        private readonly BlaBlaCarFinder blaBlaCarFinder;

        public TravelDialog(BusFinder busFinder, BlaBlaCarFinder blaBlaCarFinder) : base(nameof(TravelDialog))
        {
            this.busFinder = busFinder;
            this.blaBlaCarFinder = blaBlaCarFinder;

            AddDialog(new DateTimePrompt(nameof(DateTimePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DestinationStepAsync,
                DepartureStepAsync,
                ResultsStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DestinationStepAsync (WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            stepContext.Values[TravelInfo] = new TravelDialogModel();
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Куди їдемо?  \U0001F60A")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> DepartureStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var model = (TravelDialogModel)stepContext.Values[TravelInfo];
            model.To = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(DateTimePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Коли їдемо?  \U0001F4C5\U0001F55C\n\n(напиши у форматі день.номер_місяця.рік год:хв)"),
                RetryPrompt = MessageFactory.Text("Напиши у форматі день.номер_місяця.рік год:хв")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ResultsStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var model = (TravelDialogModel)stepContext.Values[TravelInfo];
            model.DepartureDate = DateTime.Parse(((IList<DateTimeResolution>)stepContext.Result).First().Value);
            var buses = (await busFinder.FindTripsAsync(From, model.To, model.DepartureDate)).ToList();
            var cars = (await blaBlaCarFinder.FindTripsAsync(From, model.To, model.DepartureDate)).ToList();
            if (buses.Any() || cars.Any())
            {
                var reply = new StringBuilder();
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ось що мені вдалось знайти \U0001F609"),
                    cancellationToken);
                if (buses.Any())
                {
                    reply.AppendLine("Автобуси \U0001F68C\n\n");
                    foreach (var bus in buses)
                    {
                        reply.AppendLine($"{bus.From} - {bus.To} о {bus.DepartureTime.Hours}:{bus.DepartureTime.Minutes} {Math.Ceiling(bus.Price)} грн\n\n[Купити квиток]({bus.BookingLink}) \n\n");
                    }

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(reply.ToString()),
                        cancellationToken);
                }

                if (cars.Any())
                {
                    reply.Clear();
                    reply.AppendLine("BlaBlaCar \U0001F697\n\n");
                    foreach (var car in cars)
                    {
                        reply.AppendLine($"{car.DeparturePlace.CityName} - {car.ArrivalPlace.CityName} о {car.DepartureDate:HH:mm} {car.Price.Value} грн\n\n[Забронювати]({car.Links.Front}) \n\n");
                    }

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(reply.ToString()),
                        cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("На жаль, на вказану дату мені нічого не вдалось знайти \U0001F625"),
                    cancellationToken);
            }
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
        }
    }
}
