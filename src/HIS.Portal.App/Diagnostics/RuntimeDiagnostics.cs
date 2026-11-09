namespace HIS.Portal.Diagnostics;

public sealed class RuntimeDiagnostics
{
    private readonly object _gate = new();
    private readonly List<string> _events = [];
    private int _pageCreated;
    private int _pageReleased;
    private int _webAdapterCreated;
    private int _webAdapterDestroyed;

    public event EventHandler? Changed;

    public int PageCreated => Volatile.Read(ref _pageCreated);

    public int PageReleased => Volatile.Read(ref _pageReleased);

    public int WebAdapterCreated => Volatile.Read(ref _webAdapterCreated);

    public int WebAdapterDestroyed => Volatile.Read(ref _webAdapterDestroyed);

    public IReadOnlyList<string> Events
    {
        get
        {
            lock (_gate)
                return _events.ToArray();
        }
    }

    public string Summary =>
        $"页面创建 {PageCreated} / 释放 {PageReleased} · WebView 创建 {WebAdapterCreated} / 销毁 {WebAdapterDestroyed}";

    public void RecordPageCreated(string key) => Record(
        $"PageCreated {key}",
        () => Interlocked.Increment(ref _pageCreated));

    public void RecordPageReleased(string key) => Record(
        $"PageReleased {key}",
        () => Interlocked.Increment(ref _pageReleased));

    public void RecordWebAdapterCreated(string adapter) => Record(
        $"WebAdapterCreated {adapter}",
        () => Interlocked.Increment(ref _webAdapterCreated));

    public void RecordWebAdapterDestroyed(string adapter) => Record(
        $"WebAdapterDestroyed {adapter}",
        () => Interlocked.Increment(ref _webAdapterDestroyed));

    public void RecordNote(string note) => Record(note, null);

    private void Record(string message, Action? counter)
    {
        counter?.Invoke();
        lock (_gate)
        {
            _events.Add($"{DateTime.Now:HH:mm:ss.fff} {message}");
            if (_events.Count > 500)
                _events.RemoveRange(0, _events.Count - 500);
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }
}
