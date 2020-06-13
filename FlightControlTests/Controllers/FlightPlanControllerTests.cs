using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using FlightControlWeb.DB;
using FlightControlWeb.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using FlightControlTests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers.Tests
{
    [TestClass()]
    public class FlightPlanControllerTests
    {
        public async IAsyncEnumerable<FlightPlan> GetEmptyFPList()
        {
            yield break;
        }
        [TestMethod()]
        public async Task GetFPReturn404WhenExternalServerDidntFindPlanAsync()
        {
            var fpDbMock = new Mock<IFlightPlanDB>();
            fpDbMock.Setup(x => x.LoadAllFP()).Returns(GetEmptyFPList);
            fpDbMock.Setup(x => x.IsExist("")).Returns(Task.FromResult(true));
            fpDbMock.Setup(x => x.LoadFP("")).Returns(Task.FromResult(new FlightPlan()));
            fpDbMock.Setup(x => x.SaveFP(new FlightPlan())).Returns(
                Task.FromResult(default(object)));
            fpDbMock.Setup(x => x.DeleteFlight("")).Returns(Task.FromResult(default(object)));

            var serverDbMock = new Mock<IServerDB>();
            serverDbMock.Setup(x => x.DeleteServer("")).Returns(Task.FromResult(default(object)));
            serverDbMock.Setup(x => x.SaveServer(new Server())).Returns(
                Task.FromResult(default(object)));
            serverDbMock.Setup(x => x.LoadServer(It.IsAny<string>())).Returns(
                Task.FromResult(new Server { Id = "testId", Url = "http://localhost" }));
            serverDbMock.Setup(x => x.LoadAllServers()).Returns(
                Task.FromResult(new List<Server> { new Server { Id = "Test", Url = "http://localhost" } }));

            var flightToServerDbMock = new Mock<IFlightToServerDB>();
            flightToServerDbMock.Setup(x => x.DeleteFlightToServer("")).Returns(
                Task.FromResult(default(object)));
            flightToServerDbMock.Setup(x => x.IsFlightExist("")).Returns(
                Task.FromResult(true));
            flightToServerDbMock.Setup(x => x.IsFlightExternal("")).Returns(
                Task.FromResult(false));
            flightToServerDbMock.Setup(x => x.SaveFlightToServer("", "")).Returns(
                Task.FromResult(default(object)));
            flightToServerDbMock.Setup(x => x.LoadFlightServer(It.IsAny<string>())).Returns(
                Task.FromResult("testId"));


            HttpClient httpClient = new HttpClient(new MockHttpMessageHandler("", HttpStatusCode.NotFound));


            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            FlightPlanController controller = new FlightPlanController(factoryMock.Object,
                fpDbMock.Object, serverDbMock.Object, flightToServerDbMock.Object);

            ActionResult actionResult = (await controller.GetFP("test")).Result;
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
        }

        [TestMethod()]
        public async Task GetFPFromExternalServer()
        {
            var fpDbMock = new Mock<IFlightPlanDB>();
            fpDbMock.Setup(x => x.LoadAllFP()).Returns(GetEmptyFPList);
            fpDbMock.Setup(x => x.IsExist("")).Returns(Task.FromResult(true));
            fpDbMock.Setup(x => x.LoadFP("")).Returns(Task.FromResult(new FlightPlan()));
            fpDbMock.Setup(x => x.SaveFP(new FlightPlan())).Returns(
                Task.FromResult(default(object)));
            fpDbMock.Setup(x => x.DeleteFlight("")).Returns(Task.FromResult(default(object)));

            var serverDbMock = new Mock<IServerDB>();
            serverDbMock.Setup(x => x.DeleteServer("")).Returns(Task.FromResult(default(object)));
            serverDbMock.Setup(x => x.SaveServer(new Server())).Returns(
                Task.FromResult(default(object)));
            serverDbMock.Setup(x => x.LoadServer(It.IsAny<string>())).Returns(
                Task.FromResult(new Server { Id = "testId", Url = "http://localhost" }));
            serverDbMock.Setup(x => x.LoadAllServers()).Returns(
                Task.FromResult(new List<Server> { new Server { Id = "Test", Url = "http://localhost" } }));

            var flightToServerDbMock = new Mock<IFlightToServerDB>();
            flightToServerDbMock.Setup(x => x.DeleteFlightToServer("")).Returns(
                Task.FromResult(default(object)));
            flightToServerDbMock.Setup(x => x.IsFlightExist("")).Returns(
                Task.FromResult(true));
            flightToServerDbMock.Setup(x => x.IsFlightExternal("")).Returns(
                Task.FromResult(false));
            flightToServerDbMock.Setup(x => x.SaveFlightToServer("", "")).Returns(
                Task.FromResult(default(object)));
            flightToServerDbMock.Setup(x => x.LoadFlightServer(It.IsAny<string>())).Returns(
                Task.FromResult("testId"));

            FlightPlan fp = new FlightPlan
            {
                CompanyName = "Test",
                InitLocation = new FlightPlan.InitialLocation
                {
                    Date = DateTime.Parse("2020-06-01T22:00:00Z"),
                    InitialLatitude = 33.3,
                    InitialLongitude = 51.78
                },
                Passengers = 100,
                Segments = new List<FlightPlan.Segment>
                {

                }
            };
            string message = JsonConvert.SerializeObject(fp);
            HttpClient httpClient = new HttpClient(new MockHttpMessageHandler(message, HttpStatusCode.OK));


            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            FlightPlanController controller = new FlightPlanController(factoryMock.Object,
                fpDbMock.Object, serverDbMock.Object, flightToServerDbMock.Object);

            FlightPlan actionResult = (await controller.GetFP("test")).Value;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(fp.CompanyName, actionResult.CompanyName);
            Assert.AreEqual(fp.Passengers, actionResult.Passengers);
            Assert.AreEqual(fp.Segments.ToString(), actionResult.Segments.ToString());
            Assert.AreEqual(fp.InitLocation.ToString(), actionResult.InitLocation.ToString());
        }
    }
}