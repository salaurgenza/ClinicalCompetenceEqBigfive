using ClinicalCompetence.Api.Data;
using ClinicalCompetence.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalCompetence.Api.Controllers;

[ApiController]
[Route("api/reparti/{reparto}/competenze")]
public sealed class CompetenzeController : ControllerBase
{
    private readonly IRepository _repo;

    public CompetenzeController(IRepository repo)
    {
        _repo = repo;
    }

    // GET /api/reparti/{reparto}/competenze
    [HttpGet]
    public ActionResult<IEnumerable<CompetenceArea>> GetCatalog([FromRoute] string reparto)
    {
        var cat = _repo.GetCompetenceCatalog(reparto);
        return Ok(cat);
    }

    // GET /api/reparti/{reparto}/competenze/{nurseId}
    [HttpGet("{nurseId:guid}")]
    public ActionResult<NurseEvaluation?> GetEvaluation([FromRoute] string reparto, [FromRoute] Guid nurseId)
    {
        var ev = _repo.GetCompetenceEvaluation(reparto, nurseId);
        return Ok(ev);
    }

    // POST /api/reparti/{reparto}/competenze/{nurseId}
    [HttpPost("{nurseId:guid}")]
    public ActionResult<NurseEvaluation> Upsert(
        [FromRoute] string reparto,
        [FromRoute] Guid nurseId,
        [FromBody] NurseEvaluationUpsertDto dto)
    {
        var ev = _repo.UpsertCompetenceEvaluation(reparto, nurseId, dto);
        return Ok(ev);
    }

    // GET /api/reparti/{reparto}/competenze/evals
    [HttpGet("evals")]
    public ActionResult<IEnumerable<NurseEvaluation>> GetAllEvaluations([FromRoute] string reparto)
    {
        var list = _repo.GetCompetenceEvaluationsForReparto(reparto);
        return Ok(list);
    }
}