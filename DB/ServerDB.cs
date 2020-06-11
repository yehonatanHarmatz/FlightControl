using FlightControlWeb.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.DB
{
    public class ServerDB : IServerDB
    {
        private string _connectionString;
        public ServerDB(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }
        /*
         * delete server from the db.
         */
        public async Task DeleteServer(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("DELETE FROM Servers WHERE Id = '" + id + "';", con);
            await cmd.ExecuteNonQueryAsync();
        }
        /*
         * return all the servers in the db.
         */
        public async Task<List<Server>> LoadAllServers()
        {
            List<Server> servers = new List<Server>();
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM Servers", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                Server s = new Server();
                s.Id = rdr.GetString(0);
                s.Url = rdr.GetString(1);
                servers.Add(s);
            }
            return servers;
        }
        /*
        * return server with specific id.
        */
        public async Task<Server> LoadServer(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM Servers WHERE id = '" + id + "';"
                , con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            if (rdr.Read())
            {
                Server s = new Server();
                s.Id = rdr.GetString(0);
                s.Url = rdr.GetString(1);
                return s;
            }
            return null;
        }
        /*
         * save server in the db.
         */
        public async Task SaveServer(Server server)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("INSERT INTO Servers(Id, Url)" +
                " VALUES(@Id, @Url)", con);
            cmd.Parameters.AddWithValue("@Id", server.Id);
            cmd.Parameters.AddWithValue("@Url", server.Url);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
