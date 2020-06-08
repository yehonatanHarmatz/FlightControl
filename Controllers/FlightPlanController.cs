using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using System.Net.Http;
using FlightControlWeb.DB;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private HttpClient _client;
        private readonly IFlightPlanDB _fpDb;
        private readonly IServerDB _serverDb;
        private readonly IFlightToServerDB _flightToServerDb;

        public FlightPlanController(IHttpClientFactory factory, IFlightPlanDB fpDb,
            IServerDB serverDb, IFlightToServerDB flightToServerDb)
        {
            _client = factory.CreateClient("api");
            _fpDb = fpDb;
            _serverDb = serverDb;
            _flightToServerDb = flightToServerDb;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<HttpResponseMessage>> GetFP(string id)
        {
            string serverId = await _flightToServerDb.LoadFlightServer(id);
            if (serverId != null && serverId.Equals("Not Found"))
            {
                return NotFound();
            }
            if (serverId == null)
            {
                FlightPlan fp = await _fpDb.LoadFP(id);
                if (fp == null)
                {
                    return NotFound();
                }
                return Ok(fp);
            }
            Server s = await _serverDb.LoadServer(serverId);
            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync(new string(s.Url + "/" + id));
            }
            catch (Exception)
            {
                return StatusCode(500, "cant get respone from other server");
            }
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "problem in the respone from out server");
            }
            return response;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] FlightPlan fp)
        {
            fp.Id = await GenerateIdAsync();
            if (!fp.IsValid())
            {
                return BadRequest("flight plan is invalid");
            }
            await _fpDb.SaveFP(fp);
            await _flightToServerDb.SaveFlightToServer(fp.Id, null);
            return CreatedAtAction("GetFP", new { id = fp.Id }, fp);
        }

        private async Task<string> GenerateIdAsync()
        {
            string id = new string(RandomCharsString(3) + RandomNumsString(5));
            while(await _fpDb.IsExist(id))
            {
                id = new string(RandomCharsString(3) + RandomNumsString(5));
            }
            return id;
        }
        private static string RandomCharsString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random r = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s1 => s1[r.Next(s1.Length)]).ToArray());
        }
        private static string RandomNumsString(int length)
        {
            const string nums = "0123456789";
            Random r = new Random();
            return new string(Enumerable.Repeat(nums, length)
              .Select(s1 => s1[r.Next(s1.Length)]).ToArray());
        }
    }
}