using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using FlightControlWeb.DB;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private IServerDB _serverDb;

        public ServersController(IServerDB serverDb)
        {
            _serverDb = serverDb;
        }
        [HttpGet]
        public async Task<List<Server>> GetServers()
        {
            List<Server> servers = new List<Server>();
            await foreach (Server s in _serverDb.LoadAllServers())
            {
                servers.Add(s);
            }
            return servers;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Server s)
        {
            await _serverDb.SaveServer(s);
            return CreatedAtAction(nameof(GetServers),s);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> Delete(string id)
        {
            Server s = await _serverDb.LoadServer(id);
            if (s == null)
            {
                return NotFound();
            }
            await _serverDb.DeleteServer(id);
            return s;
        }
    }
}