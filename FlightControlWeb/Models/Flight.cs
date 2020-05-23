using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Model
{
    public class Flight
    {
        public string flight_id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int passengers { get; set; }
        public string company_name { get; set; }
        public DateTime data_time { get; set; }
        public bool is_extetanl { get; set; }
    }
}
