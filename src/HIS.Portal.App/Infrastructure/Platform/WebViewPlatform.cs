using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace HIS.Portal.Infrastructure.Platform;

/// <summary>
/// Boundary for the operating-system/browser-control specifics used by the WebDemo module.
/// M1 ships only the Windows implementation and does not claim cross-platform support. A future
/// Linux/macOS adapter can implement this interface without changing the module's page code.
/// </summary>
public interface IWebViewPlatformAdapter
{
    bool IsSupported { get; }

    string Describe(NativeWebView view);

    string BuildSummary(string adapterDescription);

    bool HasNativeHandle(NativeWebView view);

    long? NativeHandle(NativeWebView view);

    IDisposable? TakeAdapterHandle(NativeWebView view);

    void DetachFromParent(NativeWebView view);
}

/// <summary>
/// Windows/Avalonia NativeWebView implementation. All direct native-handle access and adapter
/// disposal for the portal lives here.
/// </summary>
public sealed class WindowsWebViewPlatform : IWebViewPlatformAdapter
{
    public static WindowsWebViewPlatform Instance { get; } = new();

    public bool IsSupported => OperatingSystem.IsWindows();

    public string Describe(NativeWebView view) =>
        view.AdapterInfo?.ToString() ?? "NativeWebView 适配器";

    public string BuildSummary(string adapterDescription)
    {
        var type = Regex.Match(adapterDescription, @"Type\s*=\s*([^,}]+)").Groups[1].Value.Trim();
        var engine = Regex.Match(adapterDescription, @"Engine\s*=\s*([^,}]+)").Groups[1].Value.Trim();
        var version = Regex.Match(adapterDescription, @"Version\s*=\s*([^,}]+)").Groups[1].Value.Trim();
        return type.Length == 0
            ? "WebView2 适配器"
            : $"类型：{type} · 引擎：{engine} · 版本：{version}";
    }

    public bool HasNativeHandle(NativeWebView view) => view.TryGetPlatformHandle() is not null;

    public long? NativeHandle(NativeWebView view) => view.TryGetPlatformHandle()?.Handle.ToInt64();

    public IDisposable? TakeAdapterHandle(NativeWebView view) =>
        view.TryGetPlatformHandle() as IDisposable;

    public void DetachFromParent(NativeWebView view)
    {
        if (view.Parent is Panel parentPanel)
            parentPanel.Children.Remove(view);
    }
}

public static class WebViewPlatform
{
    /// <summary>M1 platform adapter. Only Windows has a verified implementation.</summary>
    public static IWebViewPlatformAdapter Current { get; } = WindowsWebViewPlatform.Instance;
}
