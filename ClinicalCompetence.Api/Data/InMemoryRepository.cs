using ClinicalCompetence.Api.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace ClinicalCompetence.Api.Data;

public sealed class InMemoryRepository : IRepository
{
    private readonly IWebHostEnvironment _env;

    private readonly Dictionary<string, List<Nurse>> _reparti = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, List<Evaluation>> _evals = new();

    // ✅ Catalogo competenze per reparto (da JSON)
    private readonly Dictionary<string, List<CompetenceArea>> _competenceCatalog =
        new(StringComparer.OrdinalIgnoreCase);

    // ✅ Valutazioni competenze per (reparto, nurseId)
    private readonly Dictionary<(string reparto, Guid nurseId), NurseEvaluation> _competenceEvals =
        new();

    public InMemoryRepository(IWebHostEnvironment env)
    {
        _env = env;

        // seed minimal (puoi anche toglierlo: i reparti verranno dal catalogo)
        _reparti["Chirurgia Generale"] = new List<Nurse>();

        LoadCompetenceCatalogFromJson();

        // ✅ opzionale ma utile: crea reparti in base al catalogo
        foreach (var reparto in _competenceCatalog.Keys)
            if (!_reparti.ContainsKey(reparto))
                _reparti[reparto] = new List<Nurse>();
    }

    private void LoadCompetenceCatalogFromJson()
    {
        try
        {
            var path = Path.Combine(_env.ContentRootPath, "wwwroot", "catalog", "competence-data.json");
            if (!File.Exists(path)) return;

            // BOM-safe (tu hai già visto l’errore BOM: qui lo preveniamo)
            var json = File.ReadAllText(path).TrimStart('\uFEFF', '\u200B');

            var data = JsonSerializer.Deserialize<Dictionary<string, List<CompetenceArea>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (data is null) return;

            _competenceCatalog.Clear();
            foreach (var kv in data)
                if (kv.Value is not null)
                    _competenceCatalog[kv.Key] = kv.Value;
        }
        catch
        {
            // se vuoi loggare, fallo qui
        }
    }

    // -------------------------
    // Reparti / Infermieri
    // -------------------------
    public IEnumerable<string> GetReparti() => _reparti.Keys.OrderBy(x => x);

    public IEnumerable<Nurse> GetNurses(string reparto)
        => _reparti.TryGetValue(reparto, out var list) ? list : Enumerable.Empty<Nurse>();

    public Nurse? GetNurse(Guid id)
        => _reparti.Values.SelectMany(x => x).FirstOrDefault(n => n.Id == id);

    public Nurse AddNurse(string reparto, NurseCreateDto dto)
    {
        if (!_reparti.TryGetValue(reparto, out var list))
        {
            list = new List<Nurse>();
            _reparti[reparto] = list;
        }

        var n = new Nurse
        {
            Id = Guid.NewGuid(),
            Reparto = reparto,
            Nome = dto.Nome.Trim(),
            Cognome = dto.Cognome.Trim(),
            Matricola = dto.Matricola?.Trim(),
            UltimaValutazione = dto.UltimaValutazione
        };

        list.Add(n);
        return n;
    }

    public Nurse? UpdateNurse(Guid id, NurseUpdateDto dto)
    {
        var n = GetNurse(id);
        if (n is null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Nome)) n.Nome = dto.Nome.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Cognome)) n.Cognome = dto.Cognome.Trim();
        if (dto.Matricola is not null) n.Matricola = dto.Matricola.Trim();
        if (dto.UltimaValutazione is not null) n.UltimaValutazione = dto.UltimaValutazione;

        return n;
    }

    public bool DeleteNurse(Guid id)
    {
        foreach (var kv in _reparti)
        {
            var idx = kv.Value.FindIndex(n => n.Id == id);
            if (idx >= 0)
            {
                kv.Value.RemoveAt(idx);
                _evals.Remove(id);

                // ✅ rimuove anche valutazioni competenze
                var keysToRemove = _competenceEvals.Keys.Where(k => k.nurseId == id).ToList();
                foreach (var k in keysToRemove) _competenceEvals.Remove(k);

                return true;
            }
        }
        return false;
    }

    // -------------------------
    // Valutazioni "vecchie"
    // -------------------------
    public Evaluation SaveEvaluation(EvaluationCreateDto dto)
    {
        var ev = new Evaluation
        {
            Id = Guid.NewGuid(),
            NurseId = dto.NurseId,
            Data = dto.Data ?? DateTimeOffset.UtcNow,
            Sections = dto.Sections ?? new List<EvaluationSection>(),
            Note = dto.Note
        };

        if (!_evals.TryGetValue(dto.NurseId, out var list))
        {
            list = new List<Evaluation>();
            _evals[dto.NurseId] = list;
        }
        list.Add(ev);

        var nurse = GetNurse(dto.NurseId);
        if (nurse is not null) nurse.UltimaValutazione = ev.Data;

        return ev;
    }

    public IEnumerable<Evaluation> GetEvaluations(Guid nurseId)
        => _evals.TryGetValue(nurseId, out var list)
            ? list.OrderByDescending(x => x.Data)
            : Enumerable.Empty<Evaluation>();

    // -------------------------
    // ✅ Competenze (nuove)
    // -------------------------
    public IEnumerable<CompetenceArea> GetCompetenceCatalog(string reparto)
        => _competenceCatalog.TryGetValue(reparto, out var list)
            ? list
            : Enumerable.Empty<CompetenceArea>();

    public NurseEvaluation? GetCompetenceEvaluation(string reparto, Guid nurseId)
    {
        _competenceEvals.TryGetValue((reparto, nurseId), out var ev);
        return ev;
    }

    public NurseEvaluation UpsertCompetenceEvaluation(string reparto, Guid nurseId, NurseEvaluationUpsertDto dto)
    {
        _competenceEvals.TryGetValue((reparto, nurseId), out var existing);

        var ev = new NurseEvaluation
        {
            Id = existing?.Id ?? Guid.NewGuid(),
            NurseId = nurseId,
            Reparto = reparto,
            Date = dto.Date ?? DateTimeOffset.UtcNow,
            ActiveAreas = dto.ActiveAreas ?? new List<bool>(),
            Selections = dto.Selections ?? new List<CompetenceSelection>(),
            MatchingLevel = dto.MatchingLevel,
            MatchingReason = dto.MatchingReason,
            CompetenceOverall = dto.CompetenceOverall,
            EqTotal = dto.EqTotal,
            BfiTotal = dto.BfiTotal
        };

        _competenceEvals[(reparto, nurseId)] = ev;

        var nurse = GetNurse(nurseId);
        if (nurse is not null) nurse.UltimaValutazione = ev.Date;

        return ev;
    }

    // ✅ QUI DENTRO la classe
    public IEnumerable<NurseEvaluation> GetCompetenceEvaluationsForReparto(string reparto)
    {
        return _competenceEvals
            .Where(kv => string.Equals(kv.Key.reparto, reparto, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Value)
            .OrderByDescending(ev => ev.CompetenceOverall ?? 0)
            .ThenByDescending(ev => ev.Date);
    }
}
