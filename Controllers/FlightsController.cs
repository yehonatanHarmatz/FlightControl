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
        public async Task<List<Flight>> GetFlights(
            [FromQuery(Name ="relative_to")] string relativTo)
        {
            bool syncAll = Request.Query.ContainsKey("sync_all");
            List<Flight> flights = new List<Flight>();
            await foreach(FlightPlan fp in _fpDb.LoadAllFP())
            {
                if (fp.IsOnAir(DateTime.Parse(relativTo).ToUniversalTime()))
                {
                    flights.Add(fp.ConvertToFlight(DateTime.Parse(relativTo).ToUniversalTime(), false));
                }
            }
            if (syncAll)
            {
                await foreach (Server s in _serverDb.LoadAllServers())
                {
                    HttpResponseMessage response = await _client.GetAsync(s.Url
                        + "/api/Flights?relative_to=" + relativTo);
                    response.EnsureSuccessStatusCode();
                    var resp = await response.Content.ReadAsStringAsync();
                    List<Flight> serverFlights = JsonConvert.DeserializeObject<List<Flight>>(resp);
                    await HandleOutFlights(serverFlights, s.Id);
                    flights.AddRange(serverFlights);
                }
            }
            return flights;
        }
        private async Task HandleOutFlights(List<Flight> flights, string serverId)
        {
            foreach (Flight f in flights)
            {
                if (f != null)
                {
                    f.IsExternal = true;
                    await _flightToServerDb.SaveFlightToServer(f.Id, serverId);
                }
            }
        }
        [HttpDelete("{id}")]
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