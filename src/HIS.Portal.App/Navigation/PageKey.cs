namespace HIS.Portal.Navigation;

/// <summary>
/// Business identity of a document. The same kind + business id must reuse the same tab.
/// </summary>
public readonly record struct PageKey(PageKind Kind, string BusinessId)
{
    public static PageKey Home { get; } = new(PageKind.Home, "home");

    public static PageKey VisitList { get; } = new(PageKind.VisitList, "visit-list");

    public static PageKey WebDemo { get; } = new(PageKind.WebDemo, "web-demo");

    public static PageKey ForVisit(string visitNo) =>
        new(PageKind.VisitDetail, (visitNo ?? string.Empty).Trim().ToUpperInvariant());

    public string Value => $"{Kind}:{BusinessId}";

    public override string ToString() => Value;
}
