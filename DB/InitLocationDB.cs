using FlightControlWeb.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    public class InitLocationDB : IInitLocationDB
    {
        private readonly string _connectionString;
        public InitLocationDB(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }
        public async Task<FlightPlan.InitialLocation> LoadInitLocation(long id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM InitLocation WHERE id = '" + id + "';", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                FlightPlan.InitialLocation init = new FlightPlan.InitialLocation();
                init.InitialLongitude = rdr.GetDouble(1);
                init.InitialLatitude = rdr.GetDouble(2);
                init.Date = rdr.GetDateTime(3).ToUniversalTime();
                return init;
            }
            return null;
        }

        public async Task<long> SaveInitLocation(FlightPlan.InitialLocation init)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("INSERT INTO InitLocation(InitialLongitude," +
                " InitialLatitude, Date) VALUES(@InitialLongitude, @InitialLatitude, @Date" +
                ")", con);
            cmd.Parameters.AddWithValue("@InitialLongitude", init.InitialLongitude);
            cmd.Parameters.AddWithValue("@InitialLatitude", init.InitialLatitude);
            cmd.Parameters.AddWithValue("@Date", init.Date);
            await cmd.ExecuteNonQueryAsync();
            using var cmd2 = new SQLiteCommand("SELECT last_insert_rowid()", con);
            Int64 lastID = (Int64)await cmd2.ExecuteScalarAsync();
            return lastID;
        }
    }
}
