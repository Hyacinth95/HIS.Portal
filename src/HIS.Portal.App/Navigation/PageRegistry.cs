using HIS.Portal.Contracts;

namespace HIS.Portal.Navigation;

/// <summary>
/// Thin navigation-facing boundary around page creation. It knows only the module factory
/// contract, so shell code never composes views or view models directly. The shell calls
/// <see cref="Create"/> only for a new PageKey; activating an existing tab never reaches here.
/// </summary>
public sealed class PageRegistry(IModuleFactory moduleFactory)
{
    private readonly IModuleFactory _moduleFactory =
        moduleFactory ?? throw new ArgumentNullException(nameof(moduleFactory));

    public bool CanCreate(PageKind kind) => _moduleFactory.CanCreate(kind);

    public IWorkspacePage Create(PageKey key)
    {
        if (!_moduleFactory.CanCreate(key.Kind))
            throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "Unsupported page kind.");

        return _moduleFactory.Create(key);
    }
}
