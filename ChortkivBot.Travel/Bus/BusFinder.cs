using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Helpers;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Core.Configuration;
using ChortkivBot.Core.Models.Travel.Bus;
using ChortkivBot.Travel.Helpers;
using Microsoft.Extensions.Options;

namespace ChortkivBot.Travel.Bus
{
    public class BusFinder
    {
        private readonly IHttpService httpService;
        private readonly IDateFormatter dateFormatter;
        private readonly ILinkBuilder linkBuilder;
        private readonly BusConfig config;
        private readonly IEnumerable<Station> availableStations;

        public BusFinder(IHttpService httpService, IDateFormatter dateFormatter, IOptions<BusConfig> options, ILinkBuilder linkBuilder)
        {
            this.httpService = httpService;
            this.dateFormatter = dateFormatter;
            this.linkBuilder = linkBuilder;
            config = options.Value;
            availableStations = GetAvailableStations().Result;
        }

        public async Task<IEnumerable<Trip>> FindTripsAsync(string from, string to, DateTime departureDate)
        {
            var fromCode = availableStations.Select(i => i).FirstOrDefault(i => i.Location == from.ToUpper())?.Code;
            var toCode = availableStations.Select(i => i).FirstOrDefault(i => i.Location == to.ToUpper())?.Code;
            var url = config.ApiUrl.Replace("{from}", fromCode).Replace("{to}", toCode)
                .Replace("{date}", dateFormatter.BusDate(departureDate));
            var headers = new WebHeaderCollection
            {
                {"Accept-Language", "ua"}
            };
            var response = await httpService.GetRequestAsync(url, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            var result = Parser.ParseTrips(responseString).ToList();
            result.ForEach(t =>
            {
                t.FromCode = fromCode;
                t.ToCode = toCode;
                t.BookingLink = linkBuilder.BuildBusLink(t);
            });
            result.RemoveAll(t => t.DepartureDate < departureDate);
            return result;
        }

        private async Task<IEnumerable<Station>> GetAvailableStations()
        {
            var headers = new WebHeaderCollection
            {
                {"Accept-Language", "ua"}
            };
            var response = await httpService.GetRequestAsync(config.SiteUrl, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            var result = Parser.ParseAllStations(responseString);
            return result;
        }
    }
}
