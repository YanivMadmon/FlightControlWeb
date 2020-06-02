using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class FlightControlWebContext : DbContext
    {

        public FlightControlWebContext(DbContextOptions<FlightControlWebContext> options)
            : base(options)
        {
        }

        public DbSet<FlightPlan> flightPlans { get; set; }
        public DbSet<Server> Servers { get; set; }
        //public DbSet<Segment> segments  { get; set; }

    }



}
