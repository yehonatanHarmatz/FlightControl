using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    public class FlightToServerDB : IFlightToServerDB
    {
        private string _connectionString;

        public FlightToServerDB(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }
        /*
         * delete flight to server from the db.
         */
        public async Task DeleteFlightToServer(string flightId)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd =
                new SQLiteCommand("DELETE FROM FlightToServer WHERE FlightId = '" + flightId
                + "';", con);
            await cmd.ExecuteNonQueryAsync();
        }
        /*
         * check if flight is from extenal srever.
         */
        public async Task<bool> IsFlightExternal(string id)
        {
            string serverId = await LoadFlightServer(id);
            return serverId != null;
        }
        /*
         * delete the server of specific flight from the db.
         */
        public async Task<string> LoadFlightServer(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM FlightToServer WHERE FlightId = '"
                + id + "';", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                if (await rdr.IsDBNullAsync(1))
                {
                    return null;
                }
                string str = rdr.GetString(1);
                return str;
            }
            return "Not Found";
        }
        /*
         * check if flight id exist in the db.
         */
        public async Task<bool> IsFlightExist(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM FlightToServer WHERE FlightId = '"
                + id + "';", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return true;
            }
            return false;
        }
        /*
         * save flight to server in the db.
         */
        public async Task SaveFlightToServer(string id, string serverID)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("INSERT INTO FlightToServer(FlightId, ServerId)" +
                " VALUES(@FlightId, @ServerId)", con);
            cmd.Parameters.AddWithValue("@FlightId", id);
            cmd.Parameters.AddWithValue("@ServerId", serverID);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
