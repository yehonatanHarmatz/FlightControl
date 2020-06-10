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
        public async Task DeleteServer(string id)
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("DELETE FROM Servers WHERE Id = '" + id + "';", con);
            await cmd.ExecuteNonQueryAsync();
        }

        public async IAsyncEnumerable<Server> LoadAllServers()
        {
            using SQLiteConnection con = new SQLiteConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SQLiteCommand("SELECT * FROM Servers", con);
            using SQLiteDataReader rdr = (SQLiteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                Server s = new Server();
                s.Id = rdr.GetString(0);
                s.Url = rdr.GetString(1);
                yield return s;
            }
        }

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
