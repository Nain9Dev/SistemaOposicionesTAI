using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Oposiciones.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("db")]
        public async Task<IActionResult> Db()
        {
            var cs = _configuration.GetConnectionString("DefaultConnection") ?? "";
            if (cs.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) || cs.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(cs);
                var userInfo = uri.UserInfo.Split(':');
                cs = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
            }
            
            using (var connection = new Npgsql.NpgsqlConnection(cs))
            {
                await connection.OpenAsync();
                using (var command = new Npgsql.NpgsqlCommand("SELECT 1", connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return Ok(new { ok = true, result });
                }
            }
        }
    }
}