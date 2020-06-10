using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class initial_location
    {
        public double latitude { get; set; }

        public double longitude { get; set; }
        public DateTime date_time { get; set; }
        //public string data_time
        //{
        //    get
        //    {
        //        return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            date = DateTime.Parse(value);
        //        }
        //    }
        //}
     


    }
}
