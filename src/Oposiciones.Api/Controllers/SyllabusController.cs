using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Interfaces;
using System.Threading.Tasks;

namespace Oposiciones.Api.Controllers
{
	[ApiController]
	[Route("api/syllabus")]
	public class SyllabusController : ControllerBase
	{
		private readonly ISyllabusRepository _syllabusRepository;

		public SyllabusController(ISyllabusRepository syllabusRepository)
		{
			_syllabusRepository = syllabusRepository;
		}

		[HttpGet("blocks")]
		public async Task<IActionResult> GetBlocks()
		{
			var blocks = await _syllabusRepository.GetBlocksAsync();
			return Ok(blocks);
		}

		[HttpGet("topics")]
		public async Task<IActionResult> GetTopicsByBlock([FromQuery] int blockId)
		{
			var topics = await _syllabusRepository.GetTopicsByBlockAsync(blockId);
			return Ok(topics);
		}
	}
}