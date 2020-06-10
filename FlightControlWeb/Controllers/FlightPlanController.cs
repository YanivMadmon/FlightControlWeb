using System;
using System.Collections.Generic;
using System.Text.Json;
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
        private IFlightPlansManager manger;
        private readonly IMemoryCache cache;
        private Dictionary<string, FlightPlan> fplist;


        public FlightPlanController(IMemoryCache memoryCache , IFlightPlansManager manager )
        {
            cache = memoryCache;
            this.manger = manager;
            fplist = cache.Get("FlightPlans") as Dictionary<string, FlightPlan>;

        }

        [HttpGet("{id}")]
        public ActionResult<FlightPlan> GetPlan(string id)
        {
            if (!fplist.ContainsKey(id))
            {
                return NotFound();
            }
            FlightPlan fp = fplist[id];
            return Ok(fp); 
        }

        [HttpPost]
        public IActionResult PostPlan(object body)
        {
            string input = body.ToString();
           
            FlightPlan newPlan = manger.createFP(input);

            if (newPlan == null||fplist.ContainsKey(newPlan.Id))
            {
                return BadRequest("worng input");
            }
            fplist.Add(newPlan.Id,newPlan);

            return Ok(newPlan);
        }

    }
}