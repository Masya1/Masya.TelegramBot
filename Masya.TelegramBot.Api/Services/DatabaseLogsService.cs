using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Masya.TelegramBot.Api.Dtos;
using Masya.TelegramBot.Api.Services.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Masya.TelegramBot.Api.Services
{
    public sealed class DatabaseLogsService : IDatabaseLogsService
    {
        public IConfiguration Configuration { get; }

        public DatabaseLogsService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(Configuration.GetConnectionString("RemoteDb"));
        }

        private IEnumerable<LogDto> MapLogsAsync(SqlDataReader reader)
        {
            var result = new List<LogDto>();
            while (reader.Read())
            {
                var dto = new LogDto
                {
                    Message = reader["Message"].ToString(),
                    Level = reader["Level"].ToString(),
                    TimeStamp = DateTime.Parse(reader["TimeStamp"].ToString())
                };
                result.Add(dto);
            }
            return result;
        }

        public async Task<IEnumerable<LogDto>> GetAgencyImportsLogsAsync(int agencyId)
        {
            using SqlConnection conn = GetConnection();
            string query = "SELECT * FROM Serilogs WHERE AgencyId = @agencyId";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@agencyId", agencyId);
            await conn.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return MapLogsAsync(reader);
        }

        public async Task<IEnumerable<LogDto>> GetBotLogsAsync()
        {
            using SqlConnection conn = GetConnection();
            string query = "SELECT * FROM Serilogs WHERE AgencyId = NULL";
            var command = new SqlCommand(query, conn);
            await conn.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return MapLogsAsync(reader);
        }
    }
}