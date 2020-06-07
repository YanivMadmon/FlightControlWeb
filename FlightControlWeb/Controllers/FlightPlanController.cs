﻿using System.Collections.Generic;
using System.Text.Json;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private FlightPlansManager manger;
        private readonly IMemoryCache cache;
        private Dictionary<string, FlightPlan> fplist;


        public FlightPlanController(IMemoryCache memoryCache )
        {
            cache = memoryCache;
            this.manger = new FlightPlansManager();
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
        public ActionResult<FlightPlan> PostPlan([FromBody] JsonElement body)
        {
            string input = body.ToString();

            dynamic obj = JsonConvert.DeserializeObject(input);

            var newPlan = new FlightPlan {
                company_name = obj["company_name"],
                passengers = obj["passengers"],
                longitude = obj["initial_location"]["longitude"],
                latitude = obj["initial_location"]["latitude"],
                data_time = obj["initial_location"]["date_time"],
                segments = new List<Segment>()
            };
            manger.idCreate(newPlan);
            manger.createSegments(newPlan , input);
            if (fplist.ContainsKey(newPlan.Id))
            {
                return NotFound();
            }
            fplist.Add(newPlan.Id,newPlan);
            return Ok(newPlan);
        }

    }
}