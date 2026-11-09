namespace HIS.Portal.Contracts;

/// <summary>
/// Read/write boundary for outpatient visits. M1 ships only an in-memory synthetic
/// implementation; a future NestJS/Oracle adapter must implement this same contract.
/// </summary>
public interface IVisitRepository
{
    /// <summary>Human-readable provenance shown by the shell (M1: synthetic demo data).</summary>
    string DataSourceNotice { get; }

    /// <summary>True only for the M1 synthetic source. Never present this as production data.</summary>
    bool IsSynthetic { get; }

    event EventHandler? Changed;

    int Count { get; }

    IReadOnlyList<VisitRecord> GetAll();

    VisitRecord? Find(string visitNo);

    VisitRecord? FindFirst();

    void Update(VisitRecord updated);
}
