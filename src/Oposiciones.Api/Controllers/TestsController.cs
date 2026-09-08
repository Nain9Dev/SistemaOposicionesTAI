using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Api.DTOs;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Api.Controllers;

[ApiController]
[Route("api/tests")]
[Produces("application/json")]
public class TestsController : ControllerBase
{
    private readonly ITestRepository _testRepository;

    public TestsController(ITestRepository testRepository)
    {
        _testRepository = testRepository;
    }

    [HttpGet("{testId:long}")]
    [ProducesResponseType(typeof(TestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long testId, CancellationToken cancellationToken)
    {
        var rows = await _testRepository.GetTestDetailRowsAsync(testId, cancellationToken);
        if (rows.Count == 0)
        {
            return NotFound();
        }

        return Ok(Compose(rows));
    }

    /// <summary>
    /// Genera un test persistido. Escribe en base de datos, por lo que exige sesion: antes
    /// cualquier peticion anonima podia crear filas en Tests y TestQuestions sin limite.
    /// </summary>
    [HttpPost("generate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate([FromBody] GenerateTestRequestDto request, CancellationToken cancellationToken)
    {
        var testId = await _testRepository.GenerateAsync(
            request.Title,
            request.SyllabusTopicId,
            request.Difficulty,
            request.TotalQuestions,
            cancellationToken);

        return Ok(new { testId });
    }

    /// <summary>
    /// Agrupa las filas planas en preguntas con sus opciones. Se recorre una sola vez y se
    /// respeta el orden que ya trae la consulta, en lugar de reordenar en memoria.
    /// </summary>
    private static TestResponseDto Compose(IReadOnlyList<TestDetailRow> rows)
    {
        var response = new TestResponseDto
        {
            TestId = rows[0].TestId,
            Title = rows[0].Title
        };

        TestQuestionDto? current = null;

        foreach (var row in rows)
        {
            if (current is null || current.QuestionId != row.QuestionId)
            {
                current = new TestQuestionDto
                {
                    QuestionId = row.QuestionId,
                    Statement = row.Statement
                };
                response.Questions.Add(current);
            }

            current.Options.Add(new TestOptionDto
            {
                Id = row.OptionId,
                SortOrder = row.SortOrder,
                Text = row.OptionText
            });
        }

        return response;
    }
}
