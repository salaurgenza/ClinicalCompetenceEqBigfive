namespace ClinicalCompetence.Api.Models;

// --------- Catalogo competenze ----------
public sealed class CompetenceArea
{
    public string Key { get; set; } = "";
    public List<string> Questions { get; set; } = new();
}

// --------- Risposte (1..4) ----------
public sealed class CompetenceSelection
{
    public int AreaIndex { get; set; }
    public int QuestionIndex { get; set; }
    public int Value { get; set; } // 1..4
}

// --------- DTO di salvataggio (dal client) ----------
public sealed class NurseEvaluationUpsertDto
{
    public DateTimeOffset? Date { get; set; }
    public List<bool>? ActiveAreas { get; set; }
    public List<CompetenceSelection>? Selections { get; set; }

    // ✅ AI/matching (dal client)
    public string? MatchingLevel { get; set; }
    public string? MatchingReason { get; set; }
    public int? CompetenceOverall { get; set; } // 100..400
    public int? EqTotal { get; set; }
    public int? BfiTotal { get; set; }
}

// --------- Valutazione salvata (server) ----------
public sealed class NurseEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NurseId { get; set; }
    public string Reparto { get; set; } = "";
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    public List<bool> ActiveAreas { get; set; } = new();
    public List<CompetenceSelection> Selections { get; set; } = new();

    // ✅ AI/matching (salvati)
    public string? MatchingLevel { get; set; }
    public string? MatchingReason { get; set; }
    public int? CompetenceOverall { get; set; } // 100..400
    public int? EqTotal { get; set; }
    public int? BfiTotal { get; set; }
}