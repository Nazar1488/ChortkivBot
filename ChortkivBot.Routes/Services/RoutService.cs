using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Core.Configuration;
using ChortkivBot.Core.Models.Rout;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChortkivBot.Routes.Services
{
    public class RoutService : IRoutService
    {
        private readonly RoutConfig routConfig;
        private readonly IHttpService httpService;
        private readonly IMemoryCache memoryCache;

        public RoutService(IOptions<RoutConfig> configuration, IHttpService httpService, IMemoryCache memoryCache)
        {
            routConfig = configuration.Value;
            this.httpService = httpService;
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Rout>> GetAvailableRoutes()
        {
            if (memoryCache.TryGetValue(CacheKeys.RoutesEntry, out RoutesData data)) return data.Routes;
            var headers = new WebHeaderCollection
            {
                {"Cookie", routConfig.Cookie}
            };
            var url = routConfig.Url.Replace("{type_id}", ((int)RequestTypeEnum.Routes).ToString());
            var response = await httpService.GetRequestAsync(url, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            data = JsonConvert.DeserializeObject<RoutesData>(responseString);

            memoryCache.Set(CacheKeys.RoutesEntry, data, TimeSpan.FromMinutes(15));

            return data.Routes;
        }

        public async Task<IEnumerable<Stop>> GetRoutStops(long routId)
        {
            var routes = await GetAvailableRoutes();
            return routes.FirstOrDefault(r => r.Id == routId)?.Stops;
        }

        public async Task<Rout> GetRoutById(long id)
        {
            var routes = await GetAvailableRoutes();
            return routes.FirstOrDefault(r => r.Id == id);
        }

        public async Task<long?> GetRoutIdByName(string name)
        {
            var routes = await GetAvailableRoutes();
            return routes.FirstOrDefault(r => r.Names.Contains(name))?.Id;
        }

        public async Task<long?> GetStopId(long routId, string name)
        {
            var stops = await GetRoutStops(routId);
            var stop = stops.FirstOrDefault(s => s.Names.Contains(name));
            return stop?.Id;
        }

        public async Task<IEnumerable<StopInfo>> GetStopInfo(long id)
        {
            var headers = new WebHeaderCollection
            {
                {"Cookie", routConfig.Cookie}
            };
            var url = routConfig.Url.Replace("{type_id}", ((int)RequestTypeEnum.StopInfo).ToString()) + $"&p={id}";
            var response = await httpService.GetRequestAsync(url, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<StopsData>(responseString);
            return data.Stops.Stops;
        }

        public async Task<Bus> GetBusById(long routId, long busId)
        {
            var headers = new WebHeaderCollection
            {
                {"Cookie", routConfig.Cookie}
            };
            var url = routConfig.Url.Replace("{type_id}", ((int)RequestTypeEnum.RoutInfo).ToString()) + $"&p={routId}";
            var response = await httpService.GetRequestAsync(url, headers);
            var responseString = await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<BusesData>(responseString);
            return data.Buses[0].Buses.FirstOrDefault(b => b.Id == busId);
        }
    }
}
