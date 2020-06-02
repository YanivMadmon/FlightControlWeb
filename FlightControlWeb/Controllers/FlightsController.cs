using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IMemoryCache cache;
        //private readonly FlightControlWebContext DB;
        private Dictionary<string, FlightPlan> fpList;
        private Dictionary<string, Server> serverList;
        private FlightsManager manager;




        public FlightsController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
            //this.DB =db ;
            this.manager = new FlightsManager();
            fpList = (Dictionary<string, FlightPlan>)cache.Get("FlightPlans");
            serverList = (Dictionary<string, Server>)cache.Get("Servers");




        }

        [HttpGet]
        public async Task<IEnumerable<Flight>> GetAllFlights([FromQuery(Name = "relative_to")] string relative_to)
        {
            DateTime relativeTime;
            if(!DateTime.TryParse(relative_to , out relativeTime))
            {
                return null; 
            }
            relativeTime = relativeTime.ToUniversalTime();
            List<Flight> flightsList = new List<Flight>();
            if (Request.Query.ContainsKey("sync_all"))
            {
                if (serverList.Count != 0)
                {
                    List<Flight> flightsListServer;

                    foreach (Server s in serverList.Values)
                    {
                        flightsListServer = (List<Flight>)await manager.serverFlights(s, relativeTime);
                        if (flightsListServer != null)
                        {
                            flightsList.AddRange(flightsListServer);

                        }
                    }

                }
            }
            foreach (FlightPlan f in fpList.Values)
            {
                manager.createFlights(f, flightsList, relativeTime);
            }
            return flightsList;

        }

        //public  Task<IEnumerable<Flight>> GetFlight([FromQuery(Name = "relative_to")] string relative_to)
        //{
        //    DateTime relativeTime;
        //    if (!DateTime.TryParse(relative_to, out relativeTime))
        //    {
        //        return null;
        //    }
        //    relativeTime = relativeTime.ToUniversalTime();
        //}
    }

    
}