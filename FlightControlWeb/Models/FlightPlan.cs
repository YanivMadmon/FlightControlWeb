using FlightControlWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Model
{
    public class FlightPlan
    {
        public string Id { get; set; } = "1";

        public int passengers { get; set; }

        public initial_location initial_location { get; set; }
    //public double longitude { get; set; }
    //public double latitude { get; set; }
    public string company_name { get; set; }
        //public DateTime data_time { get; set; }

        public List<Segment> segments { get; set; }

    }
}
