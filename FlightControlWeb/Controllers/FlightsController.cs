using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<string, FlightPlan> fpList;
        private Dictionary<string, Server> serverList;
        private FlightsManager manager;




        public FlightsController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
            this.manager = new FlightsManager(memoryCache);
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

            if (Request.Query.ContainsKey("sync_all")&&(serverList.Count != 0))
            {
                    List<Flight> flightsListServer;
                    flightsListServer = (List<Flight>)await manager.serverFlights(relativeTime);
                    if (flightsListServer != null)
                    {
                        flightsList.AddRange(flightsListServer);

                    }
            }
            foreach (FlightPlan f in fpList.Values)
            {
                manager.createFlights(f, flightsList, relativeTime);
            }
            return flightsList;

        }
    }

    
}