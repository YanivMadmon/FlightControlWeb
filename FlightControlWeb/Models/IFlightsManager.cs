using FlightControlWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightsManager
    {
        public void createFlights(FlightPlan fp, List<Flight> flightsList, DateTime relativeTime);

        public void findPlace(FlightPlan fp, List<Flight> flightsList, DateTime relativeTime);


        public  Task<List<Flight>> serverFlights(DateTime relativeTime);

        public  Task<IEnumerable<Flight>> serverGet(Server server, DateTime relativeTime);
     
    }
}
