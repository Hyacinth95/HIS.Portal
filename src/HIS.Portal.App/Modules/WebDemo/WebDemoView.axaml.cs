using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Platform;

namespace HIS.Portal.Modules.WebDemo;

public sealed record WebDemoState(
    [property: JsonPropertyName("note")] string Note,
    [property: JsonPropertyName("counter")] int Counter,
    [property: JsonPropertyName("sessionId")] string SessionId,
    [property: JsonPropertyName("patient")] string Patient,
    [property: JsonPropertyName("exam")] string Exam,
    [property: JsonPropertyName("priority")] string Priority,
    [property: JsonPropertyName("agreed")] bool Agreed);

public partial class WebDemoView : UserControl, IDisposable
{
    private readonly RuntimeDiagnostics? _diagnostics;
    private readonly IWebViewPlatformAdapter _platform;
    private readonly TaskCompletionSource<bool> _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly DispatcherTimer _initializationTimeout;
    private bool _sourceLoaded;
    private bool _adapterSeen;
    private bool _disposed;
    private bool _nativeAdapterDisposed;
    private bool _diagnosticsOpen;
    private int _adapterCreated;
    private int _adapterDestroyed;
    private string _adapterDescription = "NativeWebView";

    public WebDemoView()
        : this(null)
    {
    }

    public WebDemoView(RuntimeDiagnostics? diagnostics, IWebViewPlatformAdapter? platform = null)
    {
        _diagnostics = diagnostics;
        _platform = platform ?? WebViewPlatform.Current;
        InitializeComponent();

        _initializationTimeout = new DispatcherTimer { Interval = TimeSpan.FromSeconds(12) };
        _initializationTimeout.Tick += OnInitializationTimeout;

        Loaded += OnLoaded;
        WebView.AdapterCreated += OnAdapterCreated;
        WebView.AdapterDestroyed += OnAdapterDestroyed;
        WebView.NavigationCompleted += OnNavigationCompleted;
        WebView.NavigationStarted += OnNavigationStarted;
        WebView.NewWindowRequested += OnNewWindowRequested;

        if (DataContext is WebDemoViewModel viewModel)
            viewModel.ReloadRequested += OnReloadRequested;
        DataContextChanged += OnDataContextChanged;
    }

    public int AdapterCreatedCount => _adapterCreated;

    public int AdapterDestroyedCount => _adapterDestroyed;

    public int NavigationStartedCount { get; private set; }

    internal bool DiagnosticsOpen => _diagnosticsOpen;

    internal bool DiagnosticsPanelVisible => DiagnosticsScroll.IsVisible && DiagnosticsPanel.IsVisible;

    internal double DiagnosticsColumnWidth => WebBodyGrid.ColumnDefinitions[0].Width.Value;

    internal string DiagnosticsButtonText => DiagnosticsToggleText.Text ?? string.Empty;

    internal void SetDiagnosticsOpenForTest(bool open) => SetDiagnosticsOpen(open);

    public bool HasLiveAdapter =>
        !_disposed && !_nativeAdapterDisposed && _adapterCreated > _adapterDestroyed
        && _platform.HasNativeHandle(WebView);

    public string AdapterDescription => _adapterDescription;

    public long? NativeHandle => _platform.NativeHandle(WebView);

    public async Task WaitForReadyAsync(TimeSpan timeout)
    {
        await _ready.Task.WaitAsync(timeout);
    }

    public async Task<WebDemoState?> ReadStateAsync()
    {
        var json = await InvokeJsonAsync("JSON.stringify(window.portalDemo.getState())");
        return json is null
            ? null
            : JsonSerializer.Deserialize<WebDemoState>(json.Value.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
    }

    public async Task SetStateAsync(string note, int counter)
    {
        var noteJson = JsonSerializer.Serialize(note);
        await WebView.InvokeScript(
            $"window.portalDemo.setState({{ note: {noteJson}, counter: {counter} }})");
    }

    public async Task IncrementCounterAsync()
    {
        await WebView.InvokeScript("window.portalDemo.increment()");
    }

    public async Task<string?> ReadSessionIdAsync()
    {
        var state = await ReadStateAsync();
        return state?.SessionId;
    }

    public async Task<bool> ProbeBridgeAsync()
    {
        var result = await WebView.InvokeScript(
            "typeof window.portalDemo !== 'undefined' && !!window.portalDemo.getState");
        return string.Equals(result?.Trim().Trim('"'), "true", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string?> ExecuteScriptAsync(string script) => WebView.InvokeScript(script);

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not WebDemoViewModel viewModel)
            return;

        viewModel.ReloadRequested -= OnReloadRequested;
        viewModel.ReloadRequested += OnReloadRequested;
    }

    private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_sourceLoaded || _disposed)
            return;

        var htmlPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Web", "demo.html");
        if (!File.Exists(htmlPath))
        {
            MarkUnavailable($"未找到本地演示页面：{htmlPath}");
            return;
        }

        _sourceLoaded = true;
        ViewModel?.MarkLoading();
        _initializationTimeout.Start();
        WebView.Source = new Uri(Path.GetFullPath(htmlPath));
    }

    private void OnAdapterCreated(object? sender, WebViewAdapterEventArgs e)
    {
        if (_disposed)
            return;

        _adapterSeen = true;
        _adapterCreated++;
        _adapterDescription = _platform.Describe(WebView);
        _diagnostics?.RecordWebAdapterCreated(_adapterDescription);

        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed || ViewModel is null)
                return;

            ViewModel.AdapterState = "WebView2 已连接";
            ViewModel.AdapterDetail = $"{_platform.BuildSummary(_adapterDescription)}；正在加载本地 HTML…";
        });
    }

    private void OnAdapterDestroyed(object? sender, WebViewAdapterEventArgs e)
    {
        if (_nativeAdapterDisposed)
        {
            _diagnostics?.RecordNote("Native AdapterDestroyed event observed after explicit page-close disposal");
            return;
        }

        _nativeAdapterDisposed = true;
        _adapterDestroyed++;
        _diagnostics?.RecordWebAdapterDestroyed(_adapterDescription);

        if (_disposed)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed)
                MarkUnavailable("WebView 原生适配器已销毁。请重新打开本标签以创建新的实例。");
        });
    }

    private void OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e)
    {
        NavigationStartedCount++;
        if (!_disposed)
            Dispatcher.UIThread.Post(() => ViewModel?.MarkLoading());
    }

    private void OnToggleDiagnostics(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        SetDiagnosticsOpen(!_diagnosticsOpen);

    /// <summary>
    /// Collapsing only changes the diagnostic column width and visibility. The NativeWebView
    /// stays in its existing parent grid and keeps its page/session; it is never recreated,
    /// re-parented or navigated.
    /// </summary>
    private void SetDiagnosticsOpen(bool open)
    {
        if (_disposed || open == _diagnosticsOpen)
            return;

        _diagnosticsOpen = open;
        DiagnosticsScroll.IsVisible = open;
        DiagnosticsPanel.IsVisible = open;
        WebBodyGrid.ColumnDefinitions[0].Width = new GridLength(open ? 260 : 0);
        WebBodyGrid.ColumnSpacing = open ? 14 : 0;
        DiagnosticsToggleText.Text = open ? "收起诊断" : "展开诊断";
        ToolTip.SetTip(
            DiagnosticsToggleButton,
            open ? "收起 WebView 诊断面板" : "展开 WebView 诊断面板");
    }

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        _initializationTimeout.Stop();
        _ready.TrySetResult(true);

        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed || ViewModel is null)
                return;

            ViewModel.MarkReady($"{_platform.BuildSummary(_adapterDescription)}；已加载 Assets/Web/demo.html");
            ViewModel.DomState = "DOM 会话已就绪";
        });
    }

    private void OnInitializationTimeout(object? sender, EventArgs e)
    {
        _initializationTimeout.Stop();
        if (_adapterSeen || _disposed)
            return;

        MarkUnavailable("12 秒内未创建 WebView2 适配器。请确认已安装 Microsoft Edge WebView2 Runtime。");
    }

    private void MarkUnavailable(string detail)
    {
        _ready.TrySetException(new InvalidOperationException(detail));
        if (!_disposed)
            ViewModel?.MarkUnavailable(detail);
    }

    private void OnReloadRequested(object? sender, EventArgs e)
    {
        if (_disposed)
            return;

        ViewModel?.MarkLoading();
        if (_sourceLoaded)
            WebView.Refresh();
    }

    private void OnNewWindowRequested(object? sender, WebViewNewWindowRequestedEventArgs e)
    {
        // The prototype must not create external business windows. The local demo does not
        // use target=_blank, but cancel any unexpected new-window request defensively.
        e.Handled = true;
    }

    private WebDemoViewModel? ViewModel => DataContext as WebDemoViewModel;

    private async Task<JsonElement?> InvokeJsonAsync(string script)
    {
        var raw = await WebView.InvokeScript(script);
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var value = raw.Trim();
        if (value.StartsWith('"') && value.EndsWith('"'))
            value = JsonSerializer.Deserialize<string>(value) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value) || value == "null")
            return null;

        using var document = JsonDocument.Parse(value);
        return document.RootElement.Clone();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _initializationTimeout.Stop();
        _initializationTimeout.Tick -= OnInitializationTimeout;
        Loaded -= OnLoaded;

        WebView.AdapterCreated -= OnAdapterCreated;
        _diagnostics?.RecordNote(
            $"WebDemoView.Dispose parent={WebView.Parent?.GetType().Name ?? "null"} topLevel={TopLevel.GetTopLevel(WebView) is not null}");

        // Detach the native control host before disposing the adapter. Disposing while the
        // WebView was still parented left the WebView2 browser process alive and a reopened
        // tab could time out waiting for a fresh adapter.
        var adapterHandle = _platform.TakeAdapterHandle(WebView);
        _platform.DetachFromParent(WebView);
        DisposeNativeAdapter(adapterHandle);

        // AdapterDestroyed intentionally stays subscribed: the native adapter can be torn
        // down asynchronously after the page leaves the visual tree, and the prototype must
        // still record the real WebAdapterDestroyed evidence after disposal.
        WebView.NavigationCompleted -= OnNavigationCompleted;
        WebView.NavigationStarted -= OnNavigationStarted;
        WebView.NewWindowRequested -= OnNewWindowRequested;
        if (DataContext is WebDemoViewModel viewModel)
            viewModel.ReloadRequested -= OnReloadRequested;
        _ready.TrySetCanceled();
    }

    private void DisposeNativeAdapter(IDisposable? adapterHandle)
    {
        if (_nativeAdapterDisposed || _adapterCreated == 0)
            return;

        _nativeAdapterDisposed = true;
        try
        {
            if (adapterHandle is not null)
            {
                adapterHandle.Dispose();
                _adapterDestroyed++;
                _diagnostics?.RecordWebAdapterDestroyed(_adapterDescription + " (explicit document release)");
            }
            else
            {
                _diagnostics?.RecordNote("NativeWebView adapter handle was not disposable");
            }
        }
        catch (Exception ex)
        {
            _diagnostics?.RecordNote($"NativeWebView adapter disposal failed: {ex.Message}");
        }
    }
}
