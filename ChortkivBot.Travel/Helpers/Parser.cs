using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ChortkivBot.Core.Models.Travel.Bus;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ChortkivBot.Travel.Helpers
{
    public static class Parser
    {
        public static IEnumerable<Station> ParseAllStations(string html)
        {
            var result = new List<Station>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlBody = htmlDoc.GetElementbyId("point_from").Descendants("option").ToList();
            htmlBody.RemoveAt(0);
            foreach (var htmlNode in htmlBody)
            {
                result.Add(ParseStation(htmlNode));
            }

            return result;
        }

        private static Station ParseStation(HtmlNode html)
        {
            var name = html.InnerHtml.Replace(" ", "").Replace("\n", "");
            var code = html.Attributes["value"].Value;
            return new Station
            {
                Code = code,
                Location = name
            };
        }

        public static IEnumerable<Station> ParseStations(string json)
        {
            var start = json.IndexOf(@"""codes""", StringComparison.Ordinal);
            json = json.Remove(0, start + 8);
            var end = json.IndexOf(@",""data""", StringComparison.Ordinal);
            json = json.Remove(end, json.Length - end);
            var stations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var result = new List<Station>();
            foreach (var keyValuePair in stations)
            {
                result.Add(new Station
                {
                    Code = keyValuePair.Key,
                    Location = keyValuePair.Value
                });
            }

            return result;
        }

        public static IEnumerable<Trip> ParseTrips(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlBody = htmlDoc.DocumentNode.Descendants()
                .Where(d => d.HasClass("trip"));
            return htmlBody.Select(htmlNode => ParseTrip(htmlNode.InnerHtml)).ToList();
        }

        private static Trip ParseTrip(string html)
        {
            var result = new Trip();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var hidden = htmlDoc.DocumentNode.Descendants("span").ToArray();
            result.BusCode = hidden[1].InnerHtml;
            result.LocalPointFrom = hidden[2].InnerHtml;
            result.LocalPointTo = hidden[3].InnerHtml;
            hidden = htmlDoc.DocumentNode.Descendants("input").ToArray();
            result.RoundNum = hidden[0].Attributes["value"].Value;
            var descendants = htmlDoc.DocumentNode.Descendants("small").ToArray();
            result.From = descendants[0].InnerHtml.Replace("\n", "").Replace(" ", "");
            result.From = char.ToUpperInvariant(result.From[0]) + result.From.Substring(1).ToLower();
            result.To = descendants[2].InnerHtml.Replace("\n", "").Replace(" ", "");
            result.To = char.ToUpperInvariant(result.To[0]) + result.To.Substring(1).ToLower();
            result.BusName = descendants[4].InnerHtml.Replace("\n", "").Replace(" ", "");
            descendants = htmlDoc.DocumentNode.Descendants("b").ToArray();
            TimeSpan.TryParse(descendants[0].InnerHtml, out var departureTime);
            result.DepartureTime = departureTime;
            descendants = htmlDoc.DocumentNode.Descendants("td").Where(d => d.HasAttributes).ToArray();
            result.Date = DateTime.ParseExact(descendants[0].InnerHtml.Replace(".", "/").Trim(), "dd/MM/yy", CultureInfo.InvariantCulture);
            var temp = descendants[2].InnerText.Replace("\n", "").Replace(" ", "");
            var arrivalTime = temp.Remove(5, temp.Length - 5);
            TimeSpan.TryParse(arrivalTime, out var arrivalTimeResult);
            result.ArrivalTime = arrivalTimeResult;
            var price = descendants[3].InnerText.Replace(".", ",");
            double.TryParse(price, out var priceResult);
            result.Price = priceResult;
            var distance = descendants[4].InnerText;
            int.TryParse(distance, out var distanceResult);
            result.Distance = distanceResult;
            result.DepartureDate = result.Date.Add(result.DepartureTime);
            result.ArrivalDate = result.Date.Add(result.ArrivalTime);
            return result;
        }
    }
}
