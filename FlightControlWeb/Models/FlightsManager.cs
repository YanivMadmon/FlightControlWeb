using FlightControlWeb.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightsManager
    {
        public void createFlights(FlightPlan fp, List<Flight> flightsList, DateTime relativeTime)
        {
            int allTime = 0;
            foreach (Segment seg in fp.segments)
            {
                allTime += seg.timespan_seconds;
            }
            DateTime finalTime = fp.data_time.AddSeconds(allTime);
            if (!((fp.data_time <= relativeTime) && (relativeTime <= finalTime)))
            {
                return;
            }
            else
            {
                findPlace(fp, flightsList, relativeTime);
            }
        }
        public void findPlace(FlightPlan fp, List<Flight> flightsList, DateTime relativeTime)
        {
            DateTime initTime = fp.data_time;
            Flight newFlight = new Flight
            {
                flight_id = fp.Id,
                company_name = fp.company_name,
                passengers = fp.passengers,
                data_time = relativeTime,
                is_extetanl = true
            };
            if (!(initTime == relativeTime))
            {
                double latInit = fp.latitude;
                double lonInit = fp.longitude;
                double disTime, latForSec, lonForSec, latDate = 0, lonDate = 0;

                foreach (Segment seg in fp.segments)
                {
                    if (initTime.AddSeconds(seg.timespan_seconds) >= relativeTime)
                    {
                        disTime = (initTime - relativeTime).TotalSeconds;
                        latForSec = (seg.latitude - latInit) / seg.timespan_seconds;
                        lonForSec = (seg.longitude - lonInit) / seg.timespan_seconds;
                        latDate = latInit + (disTime * latForSec);
                        lonDate = lonInit + (disTime * lonForSec);
                        break;
                    }
                    else
                    {
                        latInit = seg.latitude;
                        lonInit = seg.longitude;
                        initTime.AddSeconds(seg.timespan_seconds);
                    }
                }
                newFlight.latitude = latDate;
                newFlight.longitude = lonDate;
            }
            else
            {
                newFlight.latitude = fp.latitude;
                newFlight.longitude = fp.longitude;
            }
            flightsList.Add(newFlight);
        }

        public async Task<IEnumerable<Flight>> serverFlights(Server server, DateTime relativeTime)
        {
            string request = server.ServerURL + "/api/Flights?relative_to=" + relativeTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string url = string.Format(request);
            WebRequest objre = WebRequest.Create(url);
            objre.Method = "GET";
            HttpWebResponse response = null;
            response = (HttpWebResponse)await objre.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            List<Flight> flightList = null;
            if (responseFromServer.Contains("flight_id")){
                flightList = JsonConvert.DeserializeObject<List<Flight>>(responseFromServer);
            }
            return flightList;
        }

    }
}
