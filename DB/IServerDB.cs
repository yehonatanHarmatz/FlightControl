using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;

namespace FlightControlWeb.DB
{
    /*
     * interface for server db
     */
    public interface IServerDB
    {
        public Task DeleteServer(string id);
        public Task SaveServer(Server server);
        public Task<List<Server>> LoadAllServers();
        public Task<Server> LoadServer(string id);



    }
}
