# Getting Started with PureSM

This guide helps you build and run PureSM and the example applications.

Prerequisites
- .NET SDK 6.0 or newer (the repository is multi-targeted; .NET 9 SDK was used for CI)
- Git (optional)

Build the repository
```powershell
# from repository root
dotnet restore
dotnet build
```

Run an example
```powershell
# Traffic light example
cd examples\TrafficLightExample
dotnet run -f net9.0 --no-build

# Order processing example
cd ..\OrderProcessingExample
dotnet run -f net9.0 --no-build

# Crawler example
cd ..\CrawlerExample
dotnet run -f net9.0 --no-build
```

Notes
- If you prefer another target framework, replace `net9.0` with `net6.0` or `net8.0`.
- If you make code changes, omit `--no-build` to allow dotnet to rebuild.

Where to look next
- `docs/usage.md` — how to use the library APIs
- `docs/examples.md` — description of each example and expected output