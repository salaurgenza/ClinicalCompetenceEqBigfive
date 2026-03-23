using ClinicalCompetence.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalCompetence.Api.Controllers;

[ApiController]
[Route("api/reparti")]
public sealed class RepartiController : ControllerBase
{
    private readonly IRepository _repo;
    public RepartiController(IRepository repo) => _repo = repo;

    [HttpGet]
    public IActionResult GetReparti() => Ok(_repo.GetReparti());

    [HttpGet("{reparto}/infermieri")]
    public IActionResult GetNurses([FromRoute] string reparto)
        => Ok(_repo.GetNurses(reparto));

    [HttpPost("{reparto}/infermieri")]
    public IActionResult AddNurse([FromRoute] string reparto, [FromBody] ClinicalCompetence.Api.Models.NurseCreateDto dto)
        => Ok(_repo.AddNurse(reparto, dto));
}