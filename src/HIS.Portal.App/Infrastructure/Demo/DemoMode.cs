namespace HIS.Portal.Infrastructure.Demo;

/// <summary>
/// Single source of truth for the M1 demo marker. It is shown in the shell status bar, the
/// visit list/detail headers and the README. M1 must never present these records as real data.
/// </summary>
public static class DemoMode
{
    public const bool IsEnabled = true;

    public const string Label = "演示模式";

    public const string DataSourceNotice = "全部为合成数据 · 不连接生产系统";

    public const string ShortBadge = "DEMO · 合成数据";
}
