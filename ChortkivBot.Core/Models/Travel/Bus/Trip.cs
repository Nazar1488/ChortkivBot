using System;

namespace ChortkivBot.Core.Models.Travel.Bus
{
    public class Trip
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string FromCode { get; set; }
        public string ToCode { get; set; }
        public string BusCode { get; set; }
        public string LocalPointFrom { get; set; }
        public string LocalPointTo { get; set; }
        public string RoundNum { get; set; }
        public string To { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int Distance { get; set; }
        public double Price { get; set; }
        public string BusName { get; set; }
        public string BookingLink { get; set; }
    }
}