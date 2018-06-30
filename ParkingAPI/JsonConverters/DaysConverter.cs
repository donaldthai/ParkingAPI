using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParkingAPI.JsonConverters
{
    internal class DaysConverter : JsonConverter
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
            var result = new List<DayOfWeek>();
            result = item.Split(',').Select(d => GetDayOfWeek(d)).ToList();

            return result;
        }

        private DayOfWeek GetDayOfWeek(string abbrievatedDay) 
        {
            DayOfWeek? result = null;
            for (var i = 0; i < DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames.Length; i++) {
                string day = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames[i];
                if (day.ToLower().Equals(abbrievatedDay.Substring(0, 3).ToLower())) {
                    result = (DayOfWeek)i;
                    break;
                }
            }

            if (result == null) {
                throw new Exception("No matching day");
            }

            return (System.DayOfWeek)result;
        }
    }

}
