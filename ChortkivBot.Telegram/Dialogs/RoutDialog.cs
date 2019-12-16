using System;
using System.Linq;
using System.Text;
using ChortkivBot.Core.Configuration;
using ChortkivBot.Services.DialogFlow;
using ChortkivBot.Services.Http;
using ChortkivBot.Travel.BlaBlaCar;
using ChortkivBot.Travel.Bus;
using ChortkivBot.Travel.Helpers;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChortkivBot.Telegram.Dialogs
{
    public class RoutDialog
    {
        private readonly ITelegramBotClient botClient;
        private readonly DialogFlowService dialogFlowService;
        private readonly BlaBlaCarFinder blaBlaCarFinder;
        private readonly BusFinder busFinder;

        public RoutDialog(ITelegramBotClient botClient)
        {
            var blaBlaCarConfig = new BlaBlaCarConfig
            {
                ApiUrl =
                    "https://public-api.blablacar.com/api/v2/trips?locale=uk_UA&cur=UAH&fn={from}&tn={to}&db={date}",
                ApiKey = "9bd3dc5d46c34a0cb1ac951322208f77"
            };
            var busConfig = new BusConfig
            {
                ApiUrl =
                    "http://ticket.bus.com.ua/order/forming_bn?point_from={from}&point_to={to}&date={date}&date_add=0&fn=round_search",
                BookingUrl =
                    "http://ticket.bus.com.ua/order/forming_bn?round_num={round_num}&count=1&email={email}&reservation=&bs_code={bus_code}&point_from={from_code}&point_to={to_code}&local_point_from={local_from_code}&local_point_to={local_to_code}&date={date}&date_add=0&date_dep={date}&fn=check_places&ya=0&submit=%D0%9F%D1%80%D0%BE%D0%B2%D0%B5%D1%80%D0%BA%D0%B0+%D0%BC%D0%B5%D1%81%D1%82+%D0%B8+%D1%81%D1%82%D0%BE%D0%B8%D0%BC%D0%BE%D1%81%D1%82%D0%B8",
                SiteUrl = "http://ticket.bus.com.ua/"
            };
            blaBlaCarFinder = new BlaBlaCarFinder(new HttpService(), new DateFormatter(), Options.Create(blaBlaCarConfig));
            busFinder = new BusFinder(new HttpService(), new DateFormatter(), Options.Create(busConfig),
                new LinkBuilder(new DateFormatter(), Options.Create(busConfig)));
            dialogFlowService = new DialogFlowService();
            this.botClient = botClient;
        }

        public async void StartDialog(DetectIntentResponse response, Chat chat)
        {
            var original = response.QueryResult.Parameters.Fields["original"];
            var userName = response.QueryResult.OutputContexts[0].Parameters.Fields["userName"];
            if (string.IsNullOrEmpty(original.StringValue))
            {

            }

            var destination = response.QueryResult.Parameters.Fields["destination"];
            if (!DateTime.TryParse(response.QueryResult.Parameters.Fields["date"].StringValue, out var date))
            {
                date = DateTime.Today;
            }

            var cars = await blaBlaCarFinder.FindTripsAsync(original.StringValue, destination.StringValue, date);
            var buses = await busFinder.FindTripsAsync(original.StringValue, destination.StringValue, date);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Ось що мені вдалось найти, {userName.StringValue} \U0001F609");
            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: stringBuilder.ToString()
            );
            stringBuilder.Clear();
            var enumerable = cars.ToList();
            if (enumerable.Any())
            {
                stringBuilder.AppendLine("BlaBlaCar \U0001F697\n\n");
                foreach (var trip in enumerable)
                {
                    stringBuilder.AppendLine("Відпарвлення: " + trip.DepartureDate);
                    stringBuilder.AppendLine("Прибуття: " + trip.ArrivalDate);
                    stringBuilder.AppendLine("Місце відправлення: " + trip.DeparturePlace.CityName);
                    stringBuilder.AppendLine("Місце прибуття: " + trip.ArrivalPlace.CityName);
                    stringBuilder.AppendLine("Ціна: " + trip.Price.Value + " грн");
                    stringBuilder.AppendLine("Кількість міць: " + trip.SeatsLeft);
                    stringBuilder.AppendLine("Автомобіль: " + trip.Car?.Make + " " + trip.Car?.Model);
                    await botClient.SendTextMessageAsync(
                        chatId: chat,
                        text: stringBuilder.ToString(),
                        replyMarkup:new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Забронювати", trip.Links.Front))
                    );
                    stringBuilder.Clear();
                }
            }

            var trips = buses.ToList();
            if (trips.Any())
            {
                stringBuilder.AppendLine("\nАвтобуси \U0001F68C\n\n");
                foreach (var trip in trips)
                {
                    stringBuilder.AppendLine("Відпарвлення: " + trip.DepartureDate);
                    stringBuilder.AppendLine("Прибуття: " + trip.ArrivalDate);
                    stringBuilder.AppendLine("Місце відправлення: " + trip.From);
                    stringBuilder.AppendLine("Місце прибуття: " + trip.To);
                    stringBuilder.AppendLine("Ціна: " + trip.Price);
                    await botClient.SendTextMessageAsync(
                        chatId: chat,
                        text: stringBuilder.ToString(),
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Забронювати", trip.BookingLink))
                    );
                    stringBuilder.Clear();
                }
            }
        }
    }
}
