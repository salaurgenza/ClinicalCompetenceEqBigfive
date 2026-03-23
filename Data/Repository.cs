using ClinicalCompetence.Api.Models;

namespace ClinicalCompetence.Api.Data;

public interface IRepository
{
    // ---- Reparti / Infermieri ----
    IEnumerable<string> GetReparti();
    IEnumerable<Nurse> GetNurses(string reparto);
    Nurse? GetNurse(Guid id);
    Nurse AddNurse(string reparto, NurseCreateDto dto);
    Nurse? UpdateNurse(Guid id, NurseUpdateDto dto);
    bool DeleteNurse(Guid id);

    // ---- Valutazioni "vecchie" (sections) ----
    Evaluation SaveEvaluation(EvaluationCreateDto dto);
    IEnumerable<Evaluation> GetEvaluations(Guid nurseId);

    // ---- Competenze (nuove) ----
    IEnumerable<CompetenceArea> GetCompetenceCatalog(string reparto);
    NurseEvaluation? GetCompetenceEvaluation(string reparto, Guid nurseId);
    NurseEvaluation UpsertCompetenceEvaluation(string reparto, Guid nurseId, NurseEvaluationUpsertDto dto);

    // ✅ elenco valutazioni competenze di un reparto (per ranking mentor)
    IEnumerable<NurseEvaluation> GetCompetenceEvaluationsForReparto(string reparto);
}