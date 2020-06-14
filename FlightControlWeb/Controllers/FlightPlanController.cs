using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private IFlightPlansManager manager;
        private readonly IMemoryCache cache;
        private Dictionary<string, FlightPlan> fpList;


        public FlightPlanController(IMemoryCache memoryCache , IFlightPlansManager manager )
        {
            //init members
            cache = memoryCache;
            this.manager = manager;
            fpList = cache.Get("FlightPlans") as Dictionary<string, FlightPlan>;

        }

        [HttpGet("{id}")]
        public async Task<object> GetPlan(string id)
        {
            //check if in cache
            FlightPlan fp;
            if (!fpList.ContainsKey(id))
            {
                //check in servers
                fp = await manager.serverFlightPlan(id);
                if (fp == null)
                {
                    return NoContent();
                }
            }
            else
            {
                fp = fpList[id];
            }
            fp.Id = id;
            return Ok(fp); 
        }

        [HttpPost]
        public IActionResult PostPlan(object body)
        {
            string input = body.ToString();
           // create flight plan from Jonson
            FlightPlan newPlan = manager.createFP(input);

            if (newPlan == null)
            {
                // bad input
                return BadRequest("worng input");
            }
            else
            {
                // check if flightplan exist
                if (fpList.ContainsKey(newPlan.Id))
                {
                    return BadRequest("Flight Plan exist");
                }
                // add to the cache
                fpList.Add(newPlan.Id, newPlan);
                return Ok(newPlan);
            }
        }

    }
}