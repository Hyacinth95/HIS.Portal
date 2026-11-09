using CommunityToolkit.Mvvm.ComponentModel;
using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;
using HIS.Portal.Navigation;

namespace HIS.Portal.Modules;

public abstract class PageViewModelBase : ObservableObject, IWorkspacePageViewModel
{
    public virtual string Title => "页面";

    public virtual bool IsDirty => false;

    public event EventHandler? CloseRequested;

    public virtual Task<bool> SaveAsync() => Task.FromResult(true);

    public virtual void DiscardChanges()
    {
    }

    public virtual void Release()
    {
    }

    protected void RequestClose() => CloseRequested?.Invoke(this, EventArgs.Empty);
}
