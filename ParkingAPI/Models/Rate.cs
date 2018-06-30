using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkingAPI.Models
{
    public class Rate
    { 
        /// <summary>
        /// Gets or sets the parking rate days.
        /// </summary>
        /// <value>The days.</value>
        [JsonConverter(typeof(JsonConverters.DaysConverter))]
        public List<DayOfWeek> Days { get; set; }

        /// <summary>
        /// Gets or sets the parking rate start and end times.
        /// </summary>
        /// <value>The times.</value>
        [JsonConverter(typeof(JsonConverters.TimesConverter))]
        public ParkingTimeRange Times { get; set; }

        /// <summary>
        /// Gets or sets the parking price.
        /// </summary>
        /// <value>The price.</value>
        public int Price { get; set; }
    }
}
