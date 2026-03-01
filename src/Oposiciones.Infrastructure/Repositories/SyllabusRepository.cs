using Dapper;
using Microsoft.Data.SqlClient;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oposiciones.Infrastructure.Repositories
{
	public class SyllabusRepository : ISyllabusRepository
	{
		private readonly string _connectionString;

		public SyllabusRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<IEnumerable<SyllabusBlock>> GetBlocksAsync()
		{
			const string sql = "SELECT Id, Code, Name FROM dbo.SyllabusBlocks ORDER BY Code;";
			using (var conn = new SqlConnection(_connectionString))
			{
				return await conn.QueryAsync<SyllabusBlock>(sql);
			}
		}

		public async Task<IEnumerable<SyllabusTopic>> GetTopicsByBlockAsync(int blockId)
		{
			const string sql = @"
SELECT Id, BlockId, TopicNumber, Title
FROM dbo.SyllabusTopics
WHERE BlockId = @BlockId
ORDER BY TopicNumber;";
			using (var conn = new SqlConnection(_connectionString))
			{
				return await conn.QueryAsync<SyllabusTopic>(sql, new { BlockId = blockId });
			}
		}
	}
}