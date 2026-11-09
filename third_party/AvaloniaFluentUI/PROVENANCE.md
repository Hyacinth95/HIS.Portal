# AvaloniaFluentUI vendored build copy

- Upstream checkout: `AvaClient/Avalonia-Fluent-UI`
- Upstream version file: `2.0.6`
- Upstream commit: `8cc9f01c1a16468fd11eeca179d2e2b0fc88ff24` (`master`)
- Copied: 2026-11-09
- Copied scope: `src/AvaloniaFluentUI/**` only, excluding `bin/`, `obj/`, `.gitignore` and the
  upstream `.csproj`; plus upstream `LICENSE`, `VERSION` and `src/SourceLink.props`.
- File count at copy time: 542 source/asset files (374 `.cs`, 163 `.axaml`, 3 `.resx`,
  1 `WinRT.idl`, 1 `Symbols.ttf`), about 4.27 MiB.

## Why a copy

`AvaloniaFluentUI.csproj` hard-codes the MicroCom generated interop target into its own source
tree:

```xml
<MicroComIdl Include="$(MSBuildThisFileDirectory)\FluentControls\Interop\WinRT\WinRT.idl"
            CSharpInteropPath="$(MSBuildThisFileDirectory)\FluentControls\Interop\WinRT\WinRT.Generated.cs" />
```

Setting only `ArtifactsPath` therefore cannot prevent a build from writing
`WinRT.Generated.cs` into the upstream checkout. The upstream checkout also already contains a
generated `WinRT.Generated.cs`. This copy uses that checked-in file and removes the generator
item, so a build never writes back to the upstream source and never needs the MicroCom MSBuild
generator package.

## Modifications in this copy

1. `AvaloniaFluentUI.csproj` is rewritten (see comments in the file): pinned package versions,
   no `MicroComIdl`, no `MicroCom.CodeGenerator.MSBuild`, no package-on-build, no SourceLink
   import.
2. No `.cs` or `.axaml` source file content is changed.
3. `SourceLink.props` is preserved verbatim, including the `EnsureTrailingSlash` workaround for
   dotnet/sdk#11105. It is not imported because this copy has no Git metadata.
4. The copy is not a Git repository; no Git command is run against it.

## Updating

The copy is intentionally a snapshot. To refresh it, re-copy only `src/AvaloniaFluentUI/**`
(without `bin`, `obj`, `.git`) from the reviewed upstream commit, replace `LICENSE`, `VERSION`
and `SourceLink.props`, and re-apply only the project-file changes above. Do not build the
upstream checkout to produce this copy.
