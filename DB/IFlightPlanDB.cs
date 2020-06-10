using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
namespace FlightControlWeb.DB
{
    public interface IFlightPlanDB
    {
        public Task<FlightPlan> LoadFP(string id);
        public IAsyncEnumerable<FlightPlan> LoadAllFP();
        public Task DeleteFlight(string id);
        public Task<Boolean> IsExist(string id);
        public Task SaveFP(FlightPlan fp);


    }
}
