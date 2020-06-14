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
            // init members
            this.cache = memoryCache;
            this.manager = manager;
            fpList = (Dictionary<string, FlightPlan>)cache.Get("FlightPlans");
            serverList = (Dictionary<string, Server>)cache.Get("Servers");
        }

        [HttpGet]
        public async Task<object> GetAllFlights([FromQuery(Name = "relative_to")] string relative_to)
        {
            DateTime relativeTime;
            //check if string can be datetime
            if(!DateTime.TryParse(relative_to , out relativeTime))
            {
                return BadRequest("worng input");
            }
            relativeTime = relativeTime.ToUniversalTime();
            List<Flight> flightsList = new List<Flight>();

            // if string contain "sync_all"
            bool syncAll = Request.Query.Keys.Contains("sync_all");
            if (syncAll && (serverList.Count != 0))
            {
                // flights in extrnal servers
                List<Flight> flightsListServer;
                flightsListServer = await manager.serverFlights(relativeTime);
                if (flightsListServer != null)
                {
                    flightsList.AddRange(flightsListServer);

                }
            }
            // flights inside our server (cache)
            foreach (FlightPlan f in fpList.Values)
            {
                manager.createFlights(f, flightsList, relativeTime);
            }

            return Ok(flightsList);

        }
        [HttpDelete("{id}")]
        public ActionResult<Flight> DeleteServer(string id)
        {
            // check if flight in the the cache
            if (!fpList.ContainsKey(id))
            {
                return NotFound();
            }
            fpList.Remove(id);

            return NoContent();
        }

    }


}