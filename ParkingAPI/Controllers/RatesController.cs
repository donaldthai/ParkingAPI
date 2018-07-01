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
        //private static readonly string jsonPath = "/Data/SampleRates1.json";
        private static readonly string jsonPath = "/Data/SampleRates2.json";
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

        private static readonly string UNAVAILABLE = "unavailable";

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
        public IActionResult Get(string startDateTime, string endDateTime)
        {
            try 
            {
                //2015-07-01T07:00:00Z to 2015-07-01T12:00:00Z
                DateTime d1 = ParseIsoDateTime(startDateTime);
                DateTime d2 = ParseIsoDateTime(endDateTime);
                //Console.WriteLine(d1);
                //Console.WriteLine(d2);

                return Ok(GetParkingRate(d1, d2)); //return price
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
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
        private string GetParkingRate(DateTime start, DateTime end) 
        {
            /*
            {
                "days": "mon,tues,thurs",
                "times": "0900-2100",
                "price": 1500
            }
            */

            if (start > end) {
                throw new ArgumentException("Start time is greater than end time.");
            }

            //1. Determine all the days from query
            // check if the days from query match up to the days of
            // the current rate we're looking at
            List<DayOfWeek> queryDays = GetQueryDays(start, end);

            //2. Determine if query time is within the time span,
            // greater than start time,
            // and less than end time

            // get rate plan
            Models.Rate curRate = GetRatePlan(queryDays, start, end);

            if (curRate == null)
            {
                Console.WriteLine("No appropriate rates found!");
                return UNAVAILABLE;
            }

            //3. Finally, if both 1 and 2 are ok, then return rate
            // otherwise, return "unavailable"


            return curRate.Price.ToString();
        }

        /// <summary>
        /// Gets the rate plan.
        /// </summary>
        /// <returns>The rate plan.</returns>
        /// <param name="queryDays">Query days.</param>
        /// <param name="d1">Start datetime.</param>
        /// <param name="d2">End datetime.</param>
        private Models.Rate GetRatePlan(List<DayOfWeek> queryDays, DateTime d1, DateTime d2) 
        {
            Models.Rate result = null;
            //look through all the available rates
            for (var i = 0; i < RATES.Rates.Count; i++) 
            {
                bool AreDaysInRate = true;
                //check if the query days are not in the plan
                for (var j = 0; j < queryDays.Count; j++)
                {
                    if (!RATES.Rates[i].Days.Contains(queryDays[j])) 
                    {
                        AreDaysInRate = false;
                        break;
                    }
                }

                //days okay
                //check time spans
                if (AreDaysInRate) {
                    //is timespan encapsulated in rate timespan
                    if (d1.TimeOfDay > RATES.Rates[i].Times.Start
                        && d2.TimeOfDay < RATES.Rates[i].Times.End)
                    {
                        result = RATES.Rates[i];
                        break;
                    }
                    //otherwise, continue
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the query days.
        /// </summary>
        /// <returns>The query days.</returns>
        /// <param name="d1">starting date.</param>
        /// <param name="d2">ending date.</param>
        private List<DayOfWeek> GetQueryDays(DateTime d1, DateTime d2) 
        {
            List<DayOfWeek> queryDays = new List<DayOfWeek>();

            //we can first check if datetime is >= 7 days
            //we can then assume that all days are included, Sun - Sat
            int numOfDays = (d2 - d1).Days;
            if (numOfDays >= 7)
            {
                queryDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
            }
            else {
                // otherwise, determine all the days from start to end datetime
                // add current day
                DayOfWeek curDay = d1.DayOfWeek;
                queryDays.Add(curDay);
                // add next days to end date
                for (int i = 0; i < numOfDays; i++) 
                {
                    curDay = GetNextDay(curDay);
                    queryDays.Add(curDay);
                }
            }

            return queryDays;
        }

        /// <summary>
        /// Gets the next day.
        /// </summary>
        /// <returns>The next day.</returns>
        /// <param name="day">Day of week.</param>
        private DayOfWeek GetNextDay(DayOfWeek day) 
        {
            int dayValue = (int)day + 1;

            //if pass enum values, set to 0
            if (dayValue >= 7) {
                dayValue = 0;
            }

            return (DayOfWeek)dayValue;
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
            DateTime result;
            try 
            {
                if (String.IsNullOrEmpty(isoString)) 
                {
                    throw new Exception("DateTime cannot be null");    
                }

                result = DateTime.ParseExact(isoString, IsoFormats,
                                       CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);    
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                throw new ArgumentException("DateTime: '" + isoString + "', " + ex.Message);
            }

            return result;
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
