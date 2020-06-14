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
        private Dictionary<string, FlightPlan> fplist;


        public FlightPlanController(IMemoryCache memoryCache , IFlightPlansManager manager )
        {
            cache = memoryCache;
            this.manager = manager;
            fplist = cache.Get("FlightPlans") as Dictionary<string, FlightPlan>;

        }

        [HttpGet("{id}")]
        public async Task<ObjectResult> GetPlan(string id)
        {
            FlightPlan fp;
            if (!fplist.ContainsKey(id))
            {
                fp = await manager.serverFlightPlan(id);
                if (fp == null)
                {
                    return BadRequest("NOT Found"); 
                }
            }
            else
            {
                fp = fplist[id];
            }
            fp.Id = id;
            return Ok(fp); 
        }

        [HttpPost]
        public IActionResult PostPlan(object body)
        {
            string input = body.ToString();
           
            FlightPlan newPlan = manager.createFP(input);

            if (newPlan == null)
            {
                return BadRequest("worng input");
            }
            else
            {
                if (fplist.ContainsKey(newPlan.Id))
                {
                    return BadRequest("Flight Plan exist");
                }
                fplist.Add(newPlan.Id, newPlan);
                return Ok(newPlan);
            }
        }

    }
}