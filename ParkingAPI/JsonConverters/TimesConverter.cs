using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParkingAPI.JsonConverters
{
    internal class TimesConverter : JsonConverter
    {
        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="value">Value.</param>
        /// <param name="serializer">Serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        /// <summary>
        /// Cans the convert.
        /// </summary>
        /// <returns><c>true</c>, if convert was caned, <c>false</c> otherwise.</returns>
        /// <param name="objectType">Object type.</param>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Models.Rate).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>
        /// ToDo: Implement validation to check for errors later.
        /// 
        /// Reads the json. Assumes json object is correct.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Serializer.</param>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string item = reader.Value as string;
            Models.ParkingTimeRange result = new Models.ParkingTimeRange();
            result = GetTimeRange(item);

            return result;
        }

        private Models.ParkingTimeRange GetTimeRange(string range) 
        {
            string[] formats = new string[] { "hhmm" };
            string[] times = range.Split('-');
            TimeSpan s = TimeSpan.ParseExact(times[0], formats, CultureInfo.InvariantCulture);
            TimeSpan e = TimeSpan.ParseExact(times[1], formats, CultureInfo.InvariantCulture);
            Console.WriteLine(s);
            Console.WriteLine(e);

            return new Models.ParkingTimeRange { Start = s, End = e };
        }
    }

}
