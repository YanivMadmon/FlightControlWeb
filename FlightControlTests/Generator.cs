using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightControlTests
{
    public  class Generator
    {
    public FlightPlan fakeFlightPlan()
    {
            Random random = new Random();
            FlightPlan fakePlan = new FlightPlan
            {
                company_name = "Name",
                passengers = random.Next(100, 600),
                initial_location = new initial_location{
                    longitude = random.Next(10, 60),
                    latitude = random.Next(10, 60),
                    date_time = DateTime.Now
              },
                segments = new List<Segment>()
            };
            Segment s = new Segment();
            s.latitude =fakePlan.initial_location.latitude + 5;
            s.longitude = fakePlan.initial_location.longitude + 5;
            s.timespan_seconds = random.Next(100, 500);
            fakePlan.segments.Add(s);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string Json = js.Serialize(fakePlan);
            JObject jsonElement = JObject.Parse(Json);
            return fakePlan;
    }
        public  List<Flight> fakeFlight()
        {
            Random random = new Random();
            List<Flight> fList = new List<Flight>();
            Flight fakeFlight = new Flight
            {
                flight_id = "id123",
                company_name = "Name",
                passengers = random.Next(100, 600),
                longitude = random.Next(10, 60),
                latitude = random.Next(10, 60),
                date_time = DateTime.Now,
                is_external = false
            };
            fList.Add(fakeFlight);
            return fList;
        }

    }
}
