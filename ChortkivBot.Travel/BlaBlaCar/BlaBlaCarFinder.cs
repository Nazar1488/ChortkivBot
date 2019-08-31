using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Helpers;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Core.Configuration;
using ChortkivBot.Core.Models.Travel.BlaBlaCar;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChortkivBot.Travel.BlaBlaCar
{
    public class BlaBlaCarFinder
    {
        private readonly IHttpService httpService;
        private readonly IDateFormatter dateFormatter;
        private readonly BlaBlaCarConfig config;

        public BlaBlaCarFinder(IHttpService httpService, IDateFormatter dateFormatter, IOptions<BlaBlaCarConfig> options)
        {
            this.httpService = httpService;
            this.dateFormatter = dateFormatter;
            config = options.Value;
        }

        public async Task<IEnumerable<Trip>> FindTripsAsync(string from, string to, DateTime departureDate)
        {
            var date = dateFormatter.BlaBlaCarDate(departureDate);
            var url = config.ApiUrl.Replace("{from}", from).Replace("{to}", to).Replace("{date}", date);
            var headers = new WebHeaderCollection
            {
                {"Key", config.ApiKey}
            };
            var response = await httpService.GetRequestAsync(url, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            const string format = "dd/MM/yyyy HH:mm:ss";
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            var data = JsonConvert.DeserializeObject<Trips>(responseString, dateTimeConverter);
            foreach (var trip in data.AvailableTrips)
            {
                trip.ArrivalDate = trip.DepartureDate.AddSeconds((double)trip.Duration.Value);
            }

            return data.AvailableTrips;
        }
    }
}
