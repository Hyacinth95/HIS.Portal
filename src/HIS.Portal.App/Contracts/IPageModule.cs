using HIS.Portal.Navigation;

namespace HIS.Portal.Contracts;

/// <summary>
/// One business module (Home, Outpatient, WebDemo). Modules own their view/view-model
/// composition; the navigation layer must not know their concrete types.
/// </summary>
public interface IPageModule
{
    bool CanHandle(PageKind kind);

    IWorkspacePage Create(PageKey key);
}
