using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using System.Net.Http;
using Newtonsoft.Json;
using FlightControlWeb.DB;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private HttpClient _client;
        private readonly IFlightPlanDB _fpDb;
        private readonly IServerDB _serverDb;
        private readonly IFlightToServerDB _flightToServerDb;
        public FlightsController(IHttpClientFactory factory, IFlightPlanDB fpDb,
            IServerDB serverDb, IFlightToServerDB flightToServerDb)
        {
            _client = factory.CreateClient("api");
            _fpDb = fpDb;
            _serverDb = serverDb;
            _flightToServerDb = flightToServerDb;
        }
        [HttpGet]
        /*
         * return all the flight that's on air.
         * if sync all appear in the url then the server ask for flight from external servers.
         */
        public async Task<ActionResult<List<Flight>>> GetFlights(
            [FromQuery(Name ="relative_to")] string relativTo)
        {
            Dictionary<string, string> flightToServer = new Dictionary<string, string>();
            bool syncAll = Request.Query.ContainsKey("sync_all");
            List<Flight> flights = new List<Flight>();
            await foreach(FlightPlan fp in _fpDb.LoadAllFP())
            {
                if (!fp.IsValid())
                {
                    continue;
                }
                if (fp.IsOnAir(DateTime.Parse(relativTo).ToUniversalTime()))
                {
                    flights.Add(fp.ConvertToFlight(
                        DateTime.Parse(relativTo).ToUniversalTime(), false));
                }
            }
            if (syncAll)
            {
                return await HandleExternalServers(relativTo, flightToServer, flights);
            }
            await AddFlightsToServers(flightToServer);
            return flights;
        }
        /*
         * handle the external servers.
         */
        private async Task<ActionResult<List<Flight>>> HandleExternalServers(string relativTo,
            Dictionary<string, string> flightToServer, List<Flight> flights)
        {
            foreach (Server s in await _serverDb.LoadAllServers())
            {
                HttpResponseMessage response;
                try
                {
                    response = await _client.GetAsync(s.Url
                    + "/api/Flights?relative_to=" + relativTo);
                }
                catch (Exception)
                {
                    return StatusCode(500, "cant get respone from other server");
                }
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    List<Flight> serverFlights = JsonConvert.DeserializeObject<List<Flight>>(resp);
                    if (!HandleOutFlights(serverFlights, s.Id, flightToServer))
                    {
                        return StatusCode(500, "get invalid flight from other server");
                    }
                    flights.AddRange(serverFlights);
                }
                else
                {
                    return StatusCode(500, "problem in the response of other server");
                }
            }
            await AddFlightsToServers(flightToServer);
            return flights;
        }
        /*
         * save connection between external server to is flights.
         */
        private async Task AddFlightsToServers(Dictionary<string, string> flightToServer)
        {
            foreach (KeyValuePair<string, string> entry in flightToServer)
            {
                if (!await _flightToServerDb.IsFlightExist(entry.Key))
                {
                   await _flightToServerDb.SaveFlightToServer(entry.Key, entry.Value);
                }
            }
        }
        /*
         * handle the external flights of a server.
         */
        private bool HandleOutFlights(List<Flight> flights,
            string serverId, Dictionary<string, string> flightToServer)
        {
            foreach (Flight f in flights)
            {
                if (f != null && f.IsValid())
                {
                    f.IsExternal = true;
                    flightToServer.Add(f.Id, serverId);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        [HttpDelete("{id}")]
        /*
         * delete flight plan from the server.
         */
        public async Task<ActionResult> Delete(string id)
        {
            FlightPlan fp = await _fpDb.LoadFP(id);
            if (fp == null)
            {
                return NotFound();
            }
            if (await _flightToServerDb.IsFlightExternal(id))
            {
                return BadRequest();
            }
            await _fpDb.DeleteFlight(id);
            await _flightToServerDb.DeleteFlightToServer(id);
            return Ok();
        }

    }
}