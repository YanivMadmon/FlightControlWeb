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
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlTests
{
    [TestClass]
    public class FlightPlanControllerTests
    {
        [TestMethod]
        public void successFlightPlan()
        {
            Generator gen = new Generator();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
            cache.Set("Servers", new Dictionary<string, Server>());


            FlightPlan fake = gen.fakeFlightPlan();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string Json = js.Serialize(fake);

            Mock<IFlightPlansManager> mockfp = new Mock<IFlightPlansManager>();
            mockfp.Setup(x => x.createFP(Json)).Returns(fake);

            var flightPlanController = new FlightPlanController(cache , mockfp.Object);
            var response = flightPlanController.PostPlan(Json);
            var okResult = response as OkObjectResult;
            Assert.IsNotNull(okResult);

            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void faildFlightPlan()
        {
            Generator gen = new Generator();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
            cache.Set("Servers", new Dictionary<string, Server>());

            FlightPlan fake = null;
            string json = "worng new plan";

            Mock<IFlightPlansManager> mockfp = new Mock<IFlightPlansManager>();
            mockfp.Setup(x => x.createFP(json)).Returns(fake);

            var flightPlanController = new FlightPlanController(cache, mockfp.Object);
            var response = flightPlanController.PostPlan(json);
            var badRequest = response as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("worng input", badRequest.Value);
        }
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
