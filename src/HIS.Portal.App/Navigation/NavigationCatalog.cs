using HIS.Portal.Navigation;

namespace HIS.Portal.Navigation;

/// <summary>
/// Static M1 navigation structure. Page creation is intentionally separate: this catalog only
/// describes what the menu exposes, while <see cref="HIS.Portal.Contracts.IModuleFactory"/>
/// decides how a page is built. M2 can replace the catalog with a server-driven menu without
/// touching the tab/document policy.
/// </summary>
public sealed record NavigationFeature(
    PageKey Key,
    string Title,
    string Subtitle,
    string Glyph,
    string Group);

public static class NavigationCatalog
{
    public static IReadOnlyList<NavigationFeature> Features { get; } =
    [
        new(PageKey.Home, "工作台首页", "打开待办、常用入口与最近记录", "\uE80F", "临床工作台"),
        new(PageKey.VisitList, "门诊接诊", "查询 30 条合成接诊记录并打开详情", "\uE8FD", "临床工作台"),
        new(PageKey.WebDemo, "检查申请 Web 演示", "官方 NativeWebView 加载本地 HTML", "\uE774", "临床工作台"),
    ];

    public static NavigationFeature Home => Features[0];

    public static NavigationFeature VisitList => Features[1];

    public static NavigationFeature WebDemo => Features[2];

    public static NavigationFeature? Find(PageKey key) =>
        Features.FirstOrDefault(feature => feature.Key == key);

    public static string GroupOf(PageKind kind) =>
        kind switch
        {
            PageKind.Home or PageKind.VisitList or PageKind.VisitDetail or PageKind.WebDemo
                => "临床工作台",
            _ => "其他",
        };
}
