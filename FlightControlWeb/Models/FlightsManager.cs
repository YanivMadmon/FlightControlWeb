using FlightControlWeb.Model;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Caching.Memory;
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
        private Dictionary<string, Server> serverList;

        public FlightsManager(IMemoryCache memoryCache)
        {
            serverList = (Dictionary<string, Server>)memoryCache.Get("Servers");
        }
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
                is_external = false
            };

            double latInit = fp.latitude;
            double lonInit = fp.longitude;
            double disTime, latForSec, lonForSec, latDate = 0, lonDate = 0;

            foreach (Segment seg in fp.segments)
            {
                if (initTime.AddSeconds(seg.timespan_seconds) >= relativeTime)
                {
                    // calcultet the place
                    disTime = (initTime - relativeTime).TotalSeconds;
                    latForSec = (seg.latitude - latInit) / seg.timespan_seconds;
                    lonForSec = (seg.longitude - lonInit) / seg.timespan_seconds;
                    latDate = latInit + (disTime * latForSec);
                    lonDate = lonInit + (disTime * lonForSec);
                    break;
                }
                else
                {
                    // go to the end of the segment
                    latInit = seg.latitude;
                    lonInit = seg.longitude;
                    initTime.AddSeconds(seg.timespan_seconds);
                }
            }
            newFlight.latitude = latDate;
            newFlight.longitude = lonDate;

            flightsList.Add(newFlight);
        }
        public async Task<IEnumerable<Flight>> serverFlights( DateTime relativeTime)
        {
            List<Flight> allFlights = new List<Flight>() ;
            List<Flight> flightsListServer;

            foreach (Server s in serverList.Values)
            {
                flightsListServer = (List<Flight>)await serverGet(s, relativeTime);
                if (flightsListServer != null)
                {
                    allFlights.AddRange(flightsListServer);
                }
            }
            return allFlights;
        }
        public async Task<IEnumerable<Flight>> serverGet(Server server, DateTime relativeTime)
        {
            HttpWebResponse response = null;

            try
            {
                string request = server.ServerURL + "/api/Flights?relative_to=" + relativeTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string url = string.Format(request);
                WebRequest objre = WebRequest.Create(url);
                objre.Method = "GET";
                // get response from the extrnal server

                response = (HttpWebResponse)await objre.GetResponseAsync();
            }
            catch (Exception e)
            {
                return null;
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            // makeing new list of flight
            List<Flight> flightList = null;
            if (responseFromServer.Contains("flight_id")){
                flightList = JsonConvert.DeserializeObject<List<Flight>>(responseFromServer);
            }
            foreach (Flight f in flightList)
            {
                f.is_external = true;
            }
            return flightList;
        }

    }
}
