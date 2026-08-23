using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Oposiciones.Domain.Interfaces;
using System.Threading.Tasks;
using System;

namespace Oposiciones.Api.Controllers
{
	[ApiController]
	[Route("api/syllabus")]
	public class SyllabusController : ControllerBase
	{
		private readonly ISyllabusRepository _syllabusRepository;
		private readonly IMemoryCache _cache;

		public SyllabusController(ISyllabusRepository syllabusRepository, IMemoryCache cache)
		{
			_syllabusRepository = syllabusRepository;
			_cache = cache;
		}

		[HttpGet("blocks")]
		public async Task<IActionResult> GetBlocks()
		{
			if (!_cache.TryGetValue("syllabus_blocks", out var blocks))
			{
				blocks = await _syllabusRepository.GetBlocksAsync();
				_cache.Set("syllabus_blocks", blocks, TimeSpan.FromHours(24));
			}
			return Ok(blocks);
		}

		[HttpGet("topics")]
		public async Task<IActionResult> GetTopicsByBlock([FromQuery] int blockId)
		{
			string cacheKey = $"syllabus_topics_{blockId}";
			if (!_cache.TryGetValue(cacheKey, out var topics))
			{
				topics = await _syllabusRepository.GetTopicsByBlockAsync(blockId);
				_cache.Set(cacheKey, topics, TimeSpan.FromHours(24));
			}
			return Ok(topics);
		}
	}
}