using FlightControlWeb.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlansManager : IFlightPlansManager
    {
        private Dictionary<string, Server> serverList;
        public FlightPlansManager(IMemoryCache memoryCache)
        {
            serverList = (Dictionary<string, Server>)memoryCache.Get("Servers");
        }

        public void idCreate(FlightPlan fp)
        {
            Random rand = new Random();
            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            string seg = fp.segments.Count.ToString();
            string company = fp.company_name;
            string newId = "";
            int i;
            newId += company[0];
            for (i = 0; i < 3; i++)
            {
                newId += time[rand.Next(1, 50) % time.Length];
            }
            newId += company[company.Length - 1];
            newId += seg;
            fp.Id = newId;
        }

        public void createSegments(FlightPlan fp, string segments_input)
        {
            int index, i1, i2, i3, first, second;
            int index2, index3;
            index = segments_input.IndexOf("segments");
            string cut_input = segments_input.Substring(index, segments_input.Length - index);
            string longitude, latitude, timespan_seconds;
            string longitu = "longitude: ", latitu = "latitude: ", timespan = "timespan_seconds: ";
            index = 0;
            cut_input = removeChars(cut_input);

            while (index < cut_input.IndexOf("]"))
            {
                index2 = cut_input.IndexOf("{", index);
                index3 = cut_input.IndexOf("}", index);
                string cut_segment = cut_input.Substring(index2, index3 - index2 + 1);

                cut_segment = removeChars(cut_segment);

                i1 = cut_segment.IndexOf("longitude");
                i2 = cut_segment.IndexOf("latitude");
                i3 = cut_segment.IndexOf("timespan_seconds");
                if (i1 == -1 || i2 == -1 || i3 == -1)
                {
                }
                i1 += longitu.Length;
                i2 += latitu.Length;
                i3 += timespan.Length;
                first = cut_segment.IndexOf(",");
                second = cut_segment.IndexOf(",", first + 1);
                longitude = cut_segment.Substring(i1, first - i1);
                latitude = cut_segment.Substring(i2, second - i2);
                timespan_seconds = cut_segment.Substring(i3, cut_segment.IndexOf("}") - i3);
                Segment s = new Segment
                {
                    longitude = Convert.ToDouble(longitude),
                    latitude = Convert.ToDouble(latitude),
                    timespan_seconds = Convert.ToInt32(timespan_seconds)
                };
                fp.segments.Add(s);
                index = index3 + 1;
            }

        }
        public String removeChars(string str)
        {
            str = str.Replace(" ", string.Empty);
            str = str.Replace("\n", string.Empty);
            str = str.Replace("\r", string.Empty);
            return str;

        }
        public FlightPlan createFP(string input)
        {
            dynamic obj = JsonConvert.DeserializeObject(input);
            if (!checkDate(input))
            {
                return null;
            }
            FlightPlan newPlan;
            try
            {
                newPlan = new FlightPlan
                {
                    company_name = obj["company_name"],
                    passengers = obj["passengers"],
                    initial_location = new initial_location
                    {
                        longitude = obj["initial_location"]["longitude"],
                        latitude = obj["initial_location"]["latitude"],
                        date_time = obj["initial_location"]["date_time"]
                    },
                    segments = new List<Segment>()

                };
            }
            catch (Exception e)
            {
                return null;
            }
            idCreate(newPlan);
            createSegments(newPlan, input);
            return newPlan;
        }
        public async Task<FlightPlan> serverFlightPlan(string id)
        {
            FlightPlan flightPlanServer;

            foreach (Server s in serverList.Values)
            {
                flightPlanServer = await serverGet(s, id);
                if (flightPlanServer != null)
                {
                    return flightPlanServer;
                }
            }
            return null;

        }
        public async Task<FlightPlan> serverGet(Server server, string id)
        {
            HttpWebResponse response;
            FlightPlan flightList = null;

            try
            {
                string request = server.ServerURL + "/api/FlightPlan/" + id;
                string url = string.Format(request);
                WebRequest objre = WebRequest.Create(url);
                objre.Method = "GET";
                // get response from the extrnal server

                response = (HttpWebResponse)await objre.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                // makeing new list of flight
                if (checkInput(responseFromServer))
                {
                    flightList = JsonConvert.DeserializeObject<FlightPlan>(responseFromServer);
                }

            }
            catch (Exception e)
            {

                e.ToString();
            }
            return flightList;
        }
        public bool checkInput(string responseFromServer)
        {
            if (
                responseFromServer.Contains("company_name") &&
                responseFromServer.Contains("passengers") &&
                responseFromServer.Contains("date_time") &&
                responseFromServer.Contains("initial_location") &&
                responseFromServer.Contains("latitude") &&
                responseFromServer.Contains("longitude"))
                return true;
            else { return false; }
        }
        public bool checkDate(string input)
        {
            int i = input.IndexOf("date_time");
            int i1 = input.IndexOf("}", i);
            string date = input.Substring(i+13, 20);
            //string date = 
            if (date.Length != 20)
            {
                return false;
            }
            if (date[4] != '-' || date[7] != '-' || date[10] != 'T' || date[13] != ':' || date[16] != ':' || date[19] != 'Z')
            {
                return false;
            }
            return true;
        }
    }
}
