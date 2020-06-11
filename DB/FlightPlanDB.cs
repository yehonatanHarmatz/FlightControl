using FlightControlWeb.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    public class FlightPlanDB : IFlightPlanDB
    {
        private readonly string _connectionString;
        private readonly IInitLocationDB _initLocationDb;

        public FlightPlanDB(IConfiguration configuration, IInitLocationDB initLocationDB)
        {
            _connectionString = configuration["ConnectionString"];
            _initLocationDb = initLocationDB;
        }
        /*
         * delete flight plan from the db.
         */
        public async Task DeleteFlight(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            string initId = (await GetInitId(id)).ToString();
            using var cmd = new SQLiteCommand("DELETE FROM FlightPlans WHERE Id = '" + id
                + "';", con);
            await cmd.ExecuteNonQueryAsync();
            using var cmd2 = new SQLiteCommand("DELETE FROM InitLocation WHERE Id = '"
                + initId + "';", con);
            await cmd2.ExecuteNonQueryAsync();
        }
        /*
         * get init location id of specific flight plan.
         */
        private async Task<long> GetInitId(string fpId)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd =
                new SQLiteCommand("SELECT * FROM FlightPlans WHERE id = '" + fpId + "';"
                , con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return rdr.GetInt32(1);
            }
            return -1;
        }
        /*
         * check if flight plan exist in the db.
         */
        public async Task<bool> IsExist(string id)
        {
            FlightPlan flight = await LoadFP(id);
            return flight != null;
        }
        /*
         * return all the flight plans from the db.
         */
        public async IAsyncEnumerable<FlightPlan> LoadAllFP()
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM FlightPlans", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                FlightPlan fp = new FlightPlan();
                fp.Id = rdr.GetString(0);
                fp.InitLocation = await _initLocationDb.LoadInitLocation(rdr.GetInt32(1));
                fp.Passengers = rdr.GetInt32(2);
                fp.CompanyName = rdr.GetString(3);
                fp.Segments =
                    JsonSerializer.Deserialize<List<FlightPlan.Segment>>(rdr.GetString(4));
                yield return fp;
            }
        }
        /*
         * return flight plan with specific id from the db.
         */
        public async Task<FlightPlan> LoadFP(string id)
        {
            FlightPlan fp = null;
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM FlightPlans WHERE id = '" + id + "';"
                , con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                fp = new FlightPlan();
                fp.Id = rdr.GetString(0);
                fp.InitLocation = await _initLocationDb.LoadInitLocation(rdr.GetInt64(1));
                fp.Passengers = rdr.GetInt32(2);
                fp.CompanyName = rdr.GetString(3);
                fp.Segments = 
                    JsonSerializer.Deserialize<List<FlightPlan.Segment>>(rdr.GetString(4));
            }
            return fp;
        }
        /*
         * save flight plan in the db.
         */
        public async Task SaveFP(FlightPlan fp)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("INSERT INTO FlightPlans(Id, InitialLocationId" +
                ", Passengers, Company, Segments) VALUES(@Id, @InitialLocationId," +
                " @Passengers, @Company, @Segments)", con);
            cmd.Parameters.AddWithValue("@Id", fp.Id);
            cmd.Parameters.AddWithValue("@InitialLocationId", 
                await _initLocationDb.SaveInitLocation(fp.InitLocation));
            cmd.Parameters.AddWithValue("@Passengers", fp.Passengers);
            cmd.Parameters.AddWithValue("@Company", fp.CompanyName);
            cmd.Parameters.AddWithValue("@Segments", JsonSerializer.Serialize(fp.Segments));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
