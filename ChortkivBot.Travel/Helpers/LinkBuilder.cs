using System.Text;
using ChortkivBot.Contracts.Helpers;
using ChortkivBot.Core.Configuration;
using Microsoft.Extensions.Options;

namespace ChortkivBot.Travel.Helpers
{
    public class LinkBuilder : ILinkBuilder
    {
        private readonly BusConfig busConfig;
        private readonly IDateFormatter dateFormatter;

        public LinkBuilder(IDateFormatter dateFormatter,
            IOptions<BusConfig> busOptions)
        {
            busConfig = busOptions.Value;
            this.dateFormatter = dateFormatter;
        }

        public string BuildBlaBlaCarLink(Core.Models.Travel.BlaBlaCar.Trip trip)
        {
            return trip.Links.Front;
        }

        public string BuildBusLink(Core.Models.Travel.Bus.Trip trip)
        {
            var stringBuilder = new StringBuilder(busConfig.BookingUrl);
            stringBuilder.Replace("{email}", "gomenyuknazar@gmail.com")
                .Replace("{from_code}", trip.FromCode)
                .Replace("{to_code}", trip.ToCode)
                .Replace("{local_from_code}", trip.LocalPointFrom)
                .Replace("{local_to_code}", trip.LocalPointTo)
                .Replace("{date}", dateFormatter.BusDate(trip.DepartureDate))
                .Replace("{round_num}", trip.RoundNum)
                .Replace("{bus_code}", trip.BusCode);
            return stringBuilder.ToString();
        }
    }
}
