using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    public class RatesController : Controller
    {
        private static readonly string jsonPath = "/Data/SampleRates1.json";
        //private readonly string jsonPath = "/Data/SampleRates2.json";
        private static readonly string[] IsoFormats = {
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        private static Models.RateList _rateList = null;
        private static Models.RateList RATES
        { 
            get 
            {
                if (_rateList == null) {
                    LoadRatesJson(jsonPath);
                    return _rateList;
                }

                return _rateList;
            }
        }


        // GET api/values
        /// <summary>
        /// Get the specified startDateTime and endDateTime.
        /// DateTime query expects an ISO format.
        /// 
        /// For example:
        /// 2015-07-01T07:00:00Z
        /// 
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="startDateTime">Start date time.</param>
        /// <param name="endDateTime">End date time.</param>
        [HttpGet]
        public string Get(string startDateTime, string endDateTime)
        {
            try 
            {
                //2015-07-01T07:00:00Z to 2015-07-01T12:00:00Z
                DateTime d1 = ParseIsoDateTime(startDateTime);
                DateTime d2 = ParseIsoDateTime(endDateTime);
                //Console.WriteLine(d1);
                //Console.WriteLine(d2);

                return GetParkingRate(d1, d2); //return price
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return "unavailable";
        }

        /// <summary>
        /// Gets the parking rate for the start and end datetime query.
        /// A rate must completely encapsulate a datetime range for it to be 
        /// available.
        /// 
        /// </summary>
        /// <returns>The parking rate. Otherwise, returns "unavailable".</returns>
        /// <param name="start">Starting datetime of query.</param>
        /// <param name="end">Ending datetime of query.</param>
        private string GetParkingRate(DateTime start, DateTime end) {
            /*
            {
                "days": "mon,tues,thurs",
                "times": "0900-2100",
                "price": 1500
            },
            */

            //1. Determine all the days from query
            // check if the days from query match up to the days of
            // the current rate we're looking at

            //2. Determine if query time is within the time span,
            // greater than start time,
            // and less than end time

            //3. Finally, if both 1 and 2 are ok, then return rate
            // otherwise, return "unavailable"


            return null;
        }

        /// <summary>
        /// Parses the iso date time.
        /// 
        /// More info on DateTimeStyles: 
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
        /// </summary>
        /// <returns>The DateTime.</returns>
        /// <param name="isoString">Iso string.</param>
        private DateTime ParseIsoDateTime(string isoString) 
        {
            return DateTime.ParseExact(isoString, IsoFormats, 
                                       CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }


        /// <summary>
        /// Loads the json.
        /// </summary>
        private static void LoadRatesJson(string filePath)
        {
            string dir = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string file = dir + filePath;

            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                Models.RateList rates = JsonConvert.DeserializeObject<Models.RateList>(json);

                _rateList = rates;
            }
        }
    }
}
