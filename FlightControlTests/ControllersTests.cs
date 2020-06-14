using FlightControlWeb.Controllers;
using FlightControlWeb.Model;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlTests
{
    [TestClass]
    public class ControllersTests
    {
        private IMemoryCache cache;
        private Generator gen;
        [TestInitialize]
        public void TestInitialize()
        {
            cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
            cache.Set("FlightPlans", new Dictionary<string, FlightPlan>());
            cache.Set("Servers", new Dictionary<string, Server>());
            Dictionary<string, Server> serverList = cache.Get("Servers") as Dictionary<string, Server>;
            Server s = new Server
            {
                Id = "123",
                ServerURL = "www.t.com"

            };
            serverList.Add("123", s);
            gen = new Generator();

        }

        [TestMethod]
        public void successFlightPlan()
        {

            FlightPlan fake = gen.fakeFlightPlan();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string Json = js.Serialize(fake);

            Mock<IFlightPlansManager> mockfp = new Mock<IFlightPlansManager>();
            mockfp.Setup(x => x.createFP(Json)).Returns(fake);

            var flightPlanController = new FlightPlanController(cache , mockfp.Object);
            var response = flightPlanController.PostPlan(Json);
            var okResult = response as OkObjectResult;
            var fplan = (FlightPlan)okResult.Value;
            Assert.IsNotNull(okResult);

            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fplan.passengers, fake.passengers);

        }

        [TestMethod]
        public void faildFlightPlan()
        {

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

            List<Flight> fakeFlightList = null;
            Mock<IFlightsManager> mockfp = new Mock<IFlightsManager>();
            mockfp.Setup(x => x.serverFlights(It.IsAny<DateTime>())).ReturnsAsync(fakeFlightList);

            var flightsController = new FlightsController(cache, mockfp.Object);
            string input = "relative_to = " + time.ToString("yyyy- MM - ddTHH:mm: ssZ");
            var response = await flightsController.GetAllFlights(input);
            Assert.IsNull(response);
        }
        [TestMethod]
        public async Task successFlight()
        {

            List<Flight> fakeFlightList = gen.fakeFlight();
            Mock<IFlightsManager> mockfp = new Mock<IFlightsManager>();
            DateTime time = fakeFlightList[0].date_time;
            mockfp.Setup(x => x.serverFlights(It.IsAny<DateTime>())).ReturnsAsync(fakeFlightList);
            FlightsController flightsController = new FlightsController(cache,mockfp.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            // Add sync_all to query.
            flightsController.HttpContext.Request.QueryString = new QueryString("?sync_all");
            string input = time.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var response = await flightsController.GetAllFlights(input);
            var ob = (List<Flight>)response;

            Assert.IsNotNull(ob);
            Assert.AreEqual(fakeFlightList[0].flight_id, ob[0].flight_id);
        }


    }
}
