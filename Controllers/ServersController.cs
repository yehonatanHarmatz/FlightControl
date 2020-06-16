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
        /*
         * return all the servers that connected to the server.
         */
        public async Task<List<Server>> GetServers()
        {
            return await _serverDb.LoadAllServers();
        }
        [HttpPost]
        /*
         * save external server that arrived from a client.
         */
        public async Task<ActionResult> Post([FromBody] Server server)
        {
            if (!server.IsValid())
            {
                return BadRequest("server file is invalid");
            }
            await _serverDb.SaveServer(server);
            return CreatedAtAction(nameof(GetServers),server);
        }
        [HttpDelete("{id}")]
        /*
         * stop communicate with specific server.
         */
        public async Task<ActionResult<Server>> Delete(string id)
        {
            Server server = await _serverDb.LoadServer(id);
            if (server == null)
            {
                return NotFound();
            }
            await _serverDb.DeleteServer(id);
            return server;
        }
    }
}