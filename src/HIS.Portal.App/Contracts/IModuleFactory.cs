using HIS.Portal.Navigation;

namespace HIS.Portal.Contracts;

/// <summary>
/// Composition boundary used by <see cref="HIS.Portal.Navigation.PageRegistry"/>. The shell calls
/// this only for a new PageKey; activating an existing tab never creates a page.
/// </summary>
public interface IModuleFactory
{
    bool CanCreate(PageKind kind);

    IWorkspacePage Create(PageKey key);
}
