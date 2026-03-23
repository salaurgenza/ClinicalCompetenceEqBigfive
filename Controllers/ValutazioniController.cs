using ClinicalCompetence.Api.Data;
using ClinicalCompetence.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalCompetence.Api.Controllers;

[ApiController]
[Route("api/valutazioni")]
public sealed class ValutazioniController : ControllerBase
{
    private readonly IRepository _repo;
    public ValutazioniController(IRepository repo) => _repo = repo;

    [HttpPost]
    public IActionResult Save([FromBody] EvaluationCreateDto dto)
        => Ok(_repo.SaveEvaluation(dto));

    [HttpGet("{nurseId:guid}")]
    public IActionResult GetForNurse(Guid nurseId)
        => Ok(_repo.GetEvaluations(nurseId));
}