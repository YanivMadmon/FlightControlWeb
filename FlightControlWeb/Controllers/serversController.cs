﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class serversController : ControllerBase
    {

        private readonly IMemoryCache cache;
        private Dictionary<string, Server> serverList;


        public serversController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
            serverList = cache.Get("Servers") as Dictionary<string, Server>;

        }

        [HttpGet]
        public  ActionResult<IEnumerable<Server>> GetServer()
        {

            return serverList.Values;
        }

        [HttpPost]
        public  ActionResult<Server> PostServer([FromBody] JsonElement body)
        {
            string input = body.ToString();

            dynamic obj = JsonConvert.DeserializeObject(input);
            Server newServer;
            try
            {
                newServer = new Server
                {
                    Id = obj["ServerId"],
                    ServerURL = obj["ServerURL"],
                };
            }
            catch (Exception e)
            {
                e.ToString();
                return BadRequest("worng input"); ;
            }

            if (serverList.ContainsKey(newServer.Id)) {
                return BadRequest("server exist");
            }
            serverList.Add(newServer.Id, newServer);    
            return Ok(newServer);
        }

        [HttpDelete("{id}")]
        public  ActionResult<Server> DeleteServer(string id)
        {
            if (!serverList.ContainsKey(id))
            {
                return NotFound();
            }
            serverList.Remove(id);

            return NoContent();
        }

    }

}