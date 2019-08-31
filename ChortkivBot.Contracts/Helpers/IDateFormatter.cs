using System;

namespace ChortkivBot.Contracts.Helpers
{
    public interface IDateFormatter
    {
        string BlaBlaCarDate(DateTime date);
        string BusDate(DateTime date);
    }
}