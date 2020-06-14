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
        private Dictionary<string, FlightPlan> fpList;
        private Dictionary<string, Server> serverList;
        private IFlightsManager manager;




        public FlightsController(IMemoryCache memoryCache , IFlightsManager manager)
        {
            this.cache = memoryCache;
            this.manager = manager;
            fpList = (Dictionary<string, FlightPlan>)cache.Get("FlightPlans");
            serverList = (Dictionary<string, Server>)cache.Get("Servers");
        }

        [HttpGet]
        public async Task<object> GetAllFlights([FromQuery(Name = "relative_to")] string relative_to)
        {
            DateTime relativeTime;
            if(!DateTime.TryParse(relative_to , out relativeTime))
            {
                return null; 
            }
            relativeTime = relativeTime.ToUniversalTime();
            List<Flight> flightsList = new List<Flight>();
            bool syncAll = Request.Query.Keys.Contains("sync_all");
            if (syncAll && (serverList.Count != 0))
            {
                List<Flight> flightsListServer;
                flightsListServer = await manager.serverFlights(relativeTime);
                if (flightsListServer != null)
                {
                    flightsList.AddRange(flightsListServer);

                }
            }
            foreach (FlightPlan f in fpList.Values)
            {
                manager.createFlights(f, flightsList, relativeTime);
            }
            if (flightsList.Count == 0)
            {
                return NotFound();
            }
            return flightsList;

        }
        [HttpDelete("{id}")]
        public ActionResult<Flight> DeleteServer(string id)
        {
            if (!fpList.ContainsKey(id))
            {
                return NotFound();
            }
            fpList.Remove(id);

            return NoContent();
        }

    }


}