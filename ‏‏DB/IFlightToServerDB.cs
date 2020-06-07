using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    public interface IFlightToServerDB
    {
        public Task<string> LoadFlightServer(string id);
        public Task SaveFlightToServer(string id, string serverID);
        public Task<Boolean> IsFlightExternal(string id);
        public Task DeleteFlightToServer(string flightId);

    }
}
