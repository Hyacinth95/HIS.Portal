namespace HIS.Portal.Infrastructure.Nest;

/// <summary>
/// Recorded M1 backend candidate: `GET /api/v2/apicache` in backend-main.
/// The route is public and returns an in-memory counter object; the deployed prefix and
/// accessibility were not verified. M1 must not call it: doing so mutates the counter. This type
/// is deliberately a contract note, not an HTTP client, and carries no authentication code.
/// </summary>
public static class NestApiCacheCandidate
{
    public const string HttpMethod = "GET";

    public const string VersionedPath = "v2/apicache";

    /// <summary>Candidate path when the NestJS global prefix is the default `api`.</summary>
    public const string CandidatePath = "api/v2/apicache";

    public const string Status = "candidate-not-connected";

    public sealed record Response(int A);
}
