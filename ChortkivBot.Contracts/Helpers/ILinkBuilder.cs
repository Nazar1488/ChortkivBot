using ChortkivBot.Core.Models.Travel.Bus;

namespace ChortkivBot.Contracts.Helpers
{
    public interface ILinkBuilder
    {
        string BuildBlaBlaCarLink(Core.Models.Travel.BlaBlaCar.Trip car);

        string BuildBusLink(Trip trip);
    }
}