using System.Collections.Generic;
using System.Threading.Tasks;
using ChortkivBot.Core.Models.Rout;

namespace ChortkivBot.Contracts.Services
{
    public interface IRoutService
    {
        Task<IEnumerable<Rout>> GetAvailableRoutes();
        Task<IEnumerable<Stop>> GetRoutStops(long routId);
        Task<Rout> GetRoutById(long id);
        Task<IEnumerable<StopInfo>> GetStopInfo(long id);
        Task<Bus> GetBusById(long routId, long busId);
        Task<long?> GetRoutIdByName(string name);
        Task<long?> GetStopId(long routId, string name);
    }
}