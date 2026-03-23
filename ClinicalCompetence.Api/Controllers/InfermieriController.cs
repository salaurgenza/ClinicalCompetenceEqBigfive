using ClinicalCompetence.Api.Data;
using ClinicalCompetence.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalCompetence.Api.Controllers;

[ApiController]
[Route("api/infermieri")]
public sealed class InfermieriController : ControllerBase
{
    private readonly IRepository _repo;
    public InfermieriController(IRepository repo) => _repo = repo;

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
        => _repo.GetNurse(id) is { } n ? Ok(n) : NotFound();

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] NurseUpdateDto dto)
        => _repo.UpdateNurse(id, dto) is { } n ? Ok(n) : NotFound();

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
        => _repo.DeleteNurse(id) ? NoContent() : NotFound();
}