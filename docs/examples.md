# Examples

This folder contains three example applications that demonstrate common state-machine patterns.

TrafficLightExample
- Purpose: Demonstrates a simple sequential state machine (Red → Green → Yellow) with a terminal `Off` state after a configured number of cycles.
- Where: `examples/TrafficLightExample`
- Run:
```powershell
cd examples\TrafficLightExample
dotnet run -f net9.0 --no-build
```
- What to expect: Console output showing the three states and final summary: "Completed N full cycles".

OrderProcessingExample
- Purpose: Demonstrates conditional transitions driven by `Context` values (payment success/failure, inventory checks).
- Where: `examples/OrderProcessingExample`
- Run:
```powershell
cd examples\OrderProcessingExample
dotnet run -f net9.0 --no-build
```
- What to expect: Flow through Pending → Payment → Shipping → Delivered or failure branches depending on simulated conditions.

CrawlerExample
- Purpose: Demonstrates an iterative workflow using a URL queue and context for results aggregation.
- Where: `examples/CrawlerExample`
- Run:
```powershell
cd examples\CrawlerExample
dotnet run -f net9.0 --no-build
```
- What to expect: Simulated fetch/parse/extract cycles and a final crawl summary.

Tips
- Use the `-f` flag to run a specific target framework.
- Examples are intentionally simple and synchronous-sounding; they use `Task`-based methods and can be adapted to real async I/O.

Contributing examples
- Add a new folder under `examples/` and follow the same pattern: small console app referencing `src/PureSM` project.
- Add the project to `Examples.sln` with: `dotnet sln Examples.sln add <yourProjectPath>`