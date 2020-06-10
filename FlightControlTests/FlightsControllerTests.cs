using FlightControlWeb.Controllers;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nancy.Json;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightControlTests
{
    [TestClass]

    class FlightsControllerTests
    {
        [TestMethod]
        public async Task faildFlight()
        {
            DateTime time = DateTime.Now;
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
            cache.Set("Servers", new Dictionary<string, Server>());

            Task<List<Flight>> fakeFlightList = null;
            Mock<IFlightsManager> mockfp = new Mock<IFlightsManager>();
            mockfp.Setup(x => x.serverFlights(time)).Returns(fakeFlightList);

            var flightsController = new FlightsController(cache, mockfp.Object);
            string input = "relative_to = " + time.ToString("yyyy- MM - ddTHH:mm: ssZ");
            var response = await flightsController.GetAllFlights(input);
            var badRequest = response as Task<IEnumerable<Flight>>;
            Assert.IsNull(badRequest);
        }
        //[TestMethod]
        //public async Task successFlight()
        //{
        //    DateTime time = DateTime.Now;
        //    Generator gen = new Generator();
        //    IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        //    cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
        //    cache.Set("Servers", new Dictionary<string, Server>());


        //    Task<List<Flight>> fakeFlightList = gen.fakeFlight();
        //    Mock<IFlightsManager> mockfp = new Mock<IFlightsManager>();
        //    mockfp.Setup(x => x.serverFlights(time)).Returns(fakeFlightList);

        //    var flightsController = new FlightsController(cache, mockfp.Object);
        //    string input = time.ToString("yyyy-MM-ddTHH:mm:ssZ");
        //    var response = await flightsController.GetAllFlights(input);
        //    var ob = response as Task<IEnumerable<Flight>>;
        //    Assert.IsNotNull(ob);
        //}
    }
}
