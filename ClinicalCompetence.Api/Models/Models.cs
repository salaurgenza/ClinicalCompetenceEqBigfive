namespace ClinicalCompetence.Api.Models;

public sealed class Nurse
{
    public Guid Id { get; set; }
    public string Reparto { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string? Matricola { get; set; }
    public DateTimeOffset? UltimaValutazione { get; set; }
}

public sealed class NurseCreateDto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string? Matricola { get; set; }
    public DateTimeOffset? UltimaValutazione { get; set; }
}

public sealed class NurseUpdateDto
{
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public string? Matricola { get; set; }
    public DateTimeOffset? UltimaValutazione { get; set; }
}

public sealed class Evaluation
{
    public Guid Id { get; set; }
    public Guid NurseId { get; set; }
    public DateTimeOffset Data { get; set; }
    public List<EvaluationSection> Sections { get; set; } = new();
    public string? Note { get; set; }
}

public sealed class EvaluationSection
{
    public string Key { get; set; } = "";
    public string Title { get; set; } = "";
    public double? Average { get; set; }
    public int AnsweredCount { get; set; }
}

public sealed class EvaluationCreateDto
{
    public Guid NurseId { get; set; }
    public DateTimeOffset? Data { get; set; }
    public List<EvaluationSection>? Sections { get; set; }
    public string? Note { get; set; }
}