using FlightControlWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlansManager
    {
        FlightControlWebContext DB;
        private static Dictionary<string, FlightPlan> dic_plans = new Dictionary<string, FlightPlan>();
        private static List<FlightPlan> list_plans = new List<FlightPlan>();

        public FlightPlansManager(FlightControlWebContext db)
        {
            this.DB = db;
        }
        public List<FlightPlan> GetAllFlights()
        {
            FlightPlan f = new FlightPlan();
            FlightPlan f1 = new FlightPlan();
            list_plans.Add(f);
            list_plans.Add(f1);

            return list_plans;
        }
        public void AddFlight(FlightPlan fp)
        {
            list_plans.Add(fp);
        }
        public void DeleteFlight(string id)
        {
            //list_plans.Remove(id);
        }
    }
}
