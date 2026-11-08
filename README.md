Additional information can be found at [M Apps](https://murcex.notion.site/M-Apps-c5fc615d33b6467694929b3fec996db0)

## Building

Shared `Murcex.*` libraries are consumed via `ProjectReference` (see `Directory.Build.props`, `$(MCoreRoot)`), so a change is picked up on the next build; there is no pack/upgrade step.

- Requires the .NET 10 SDK. The target framework is set once in `Directory.Build.props` (`TargetFramework`); change that single line to upgrade.
- On Windows, build from a short path or enable long paths (Azure Functions worker extensions exceed 260 chars otherwise).
- Build everything: `dotnet build m-core.sln` (per-component `.sln` files still work).
- Publish a library package: `dotnet pack <library>.csproj -c Release` (output in `artifacts\packages`, not committed).
