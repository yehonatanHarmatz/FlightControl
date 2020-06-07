using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;

namespace FlightControlWeb.DB
{
    public interface IServerDB
    {
        public Task DeleteServer(string id);
        public Task SaveServer(Server server);
        public IAsyncEnumerable<Server> LoadAllServers();
        public Task<Server> LoadServer(string id);



    }
}
