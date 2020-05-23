using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private FlightPlansManager manger;
        private FlightControlWebContext DB;
        public FlightPlanController()
        {
           // this.DB = db;
            this.manger = new FlightPlansManager(this.DB);
        }

        [HttpGet]
        public List<FlightPlan> GetPlan(int id)
        {

            return manger.GetAllFlights();
        }

        [HttpPost]
        public void PostPlan(FlightPlan fp)
        {
            manger.AddFlight(fp);
        }

    }
}