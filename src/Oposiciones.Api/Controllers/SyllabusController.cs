using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Interfaces;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Oposiciones.Api.Controllers
{
	[ApiController]
	[Route("api/syllabus")]
	public class SyllabusController : ControllerBase
	{
		private readonly ISyllabusRepository _syllabusRepository;
		private readonly IDistributedCache _cache;

		public SyllabusController(ISyllabusRepository syllabusRepository, IDistributedCache cache)
		{
			_syllabusRepository = syllabusRepository;
			_cache = cache;
		}

		[HttpGet("blocks")]
		public async Task<IActionResult> GetBlocks()
		{
			var cachedBlocks = await _cache.GetStringAsync("syllabus_blocks");
			if (!string.IsNullOrEmpty(cachedBlocks))
			{
				return Ok(JsonSerializer.Deserialize<System.Collections.Generic.IEnumerable<Oposiciones.Domain.Entities.SyllabusBlock>>(cachedBlocks));
			}

			var blocks = await _syllabusRepository.GetBlocksAsync();
			await _cache.SetStringAsync("syllabus_blocks", JsonSerializer.Serialize(blocks), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
			
			return Ok(blocks);
		}

		[HttpGet("topics")]
		public async Task<IActionResult> GetTopicsByBlock([FromQuery] int blockId)
		{
			string cacheKey = $"syllabus_topics_{blockId}";
			var cachedTopics = await _cache.GetStringAsync(cacheKey);
			if (!string.IsNullOrEmpty(cachedTopics))
			{
				return Ok(JsonSerializer.Deserialize<System.Collections.Generic.IEnumerable<Oposiciones.Domain.Entities.SyllabusTopic>>(cachedTopics));
			}

			var topics = await _syllabusRepository.GetTopicsByBlockAsync(blockId);
			await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(topics), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
			
			return Ok(topics);
		}
	}
}