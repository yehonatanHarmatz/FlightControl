using FlightControlWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    /*
     * interface for init location db
     */
    public interface IInitLocationDB
    {
        public Task<FlightPlan.InitialLocation> LoadInitLocation(long id);
        public Task<Int64> SaveInitLocation(FlightPlan.InitialLocation init);


    }
}
