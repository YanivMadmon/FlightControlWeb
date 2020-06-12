using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using Microsoft.Extensions.Caching.Memory;
using FlightControlWeb.Model;

namespace FlightControlWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<FlightControlWebContext>(opt =>
            //  opt.UseInMemoryDatabase("FlightControlWeb"));

            services.AddSingleton<FlightsManager>();
            services.AddSingleton<FlightPlansManager>();
            services.AddScoped<IFlightPlansManager, FlightPlansManager>();
            services.AddScoped<IFlightsManager, FlightsManager>();

            services.AddControllers();
            services.AddMemoryCache();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
            cache.Set("Servers", new Dictionary<string, Server>());
           

            app.UseStaticFiles();
            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
