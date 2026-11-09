using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;
using HIS.Portal.Navigation;

namespace HIS.Portal.Modules.WebDemo;

public sealed partial class WebDemoViewModel : PageViewModelBase
{
    public WebDemoViewModel()
    {
        ReloadCommand = new RelayCommand(() => ReloadRequested?.Invoke(this, EventArgs.Empty));
    }

    public override string Title => "检查申请 · Web 演示";

    public IRelayCommand ReloadCommand { get; }

    public event EventHandler? ReloadRequested;

    [ObservableProperty]
    private string _adapterState = "正在初始化 WebView2…";

    [ObservableProperty]
    private string _adapterDetail = "等待官方 NativeWebView 适配器创建";

    [ObservableProperty]
    private string _domState = "尚未读取 DOM";

    [ObservableProperty]
    private bool _isWebReady;

    [ObservableProperty]
    private bool _isUnavailable;

    public void MarkLoading()
    {
        IsUnavailable = false;
        IsWebReady = false;
        AdapterState = "正在初始化 WebView2…";
        AdapterDetail = "正在创建官方 NativeWebView 适配器";
    }

    public void MarkReady(string adapterDetail)
    {
        IsUnavailable = false;
        IsWebReady = true;
        AdapterState = "WebView2 已连接";
        AdapterDetail = adapterDetail;
    }

    public void MarkUnavailable(string detail)
    {
        IsWebReady = false;
        IsUnavailable = true;
        AdapterState = "WebView2 不可用";
        AdapterDetail = detail;
    }
}
