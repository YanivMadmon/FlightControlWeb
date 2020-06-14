using FlightControlWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightPlansManager
    {
        public void idCreate(FlightPlan fp);

        public void createSegments(FlightPlan fp, string segments_input);

        public string removeChars(string str);
        public FlightPlan createFP(string input);
        public  Task<FlightPlan> serverFlightPlan(string id);
        public  Task<FlightPlan> serverGet(Server server, string id);
        public bool checkDate(string date);

    }
}
