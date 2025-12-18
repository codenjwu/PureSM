# PureSM

PureSM is a small, dependency-free C# library for building explicit state machines with a fluent builder API. It demonstrates state lifecycle hooks (Entry/Action/Exit), transitions (with async conditions), context-passing, and terminal (end) states.

**Features**
- Lightweight state and transition model
- Async-friendly (`Task`-based lifecycle and conditions)
- Context object for passing data between states
- Fluent `StateMachineBuilder` for easy construction
- Example projects demonstrating common patterns

**Contents**
- `src/PureSM` — library sources
- `tests` — MSTest unit tests project (`PureSM.Tests`)
- `examples` — sample applications demonstrating usage:
  - `TrafficLightExample` (previously `SimpleExample`) — simple traffic-light with a terminal Off state
  - `OrderProcessingExample` (previously `AdvancedExample`) — order workflow with conditional transitions
  - `CrawlerExample` — iterative crawler-like workflow using a queue
- `docs` — additional documentation (if present)

**Getting started**
Prerequisites:
- .NET SDK 6.0+ (9.0 SDK used for multi-target builds in CI)

Build the library and examples:

```powershell
# from repository root
dotnet restore
dotnet build
```

Run an example (choose one):

```powershell
cd examples\TrafficLightExample
dotnet run -f net9.0 --no-build

cd ..\OrderProcessingExample
dotnet run -f net9.0 --no-build

cd ..\CrawlerExample
dotnet run -f net9.0 --no-build
```

If you want to run for another target framework (e.g. `net6.0`), change the `-f` flag accordingly.

**Usage (library overview)**
- Create a `Context` and set any initial data via `context.SetItem(key, object)`.
- Implement `State` subclasses and override `Entry()`, `Action()`, `Exit()` as `Task<State>` returning the state instance.
- Create `Transition` instances with async condition functions and target state lists.
- Register transitions on states via `state.AddTransition(transition)`.
- Build the state machine via `new StateMachineBuilder().AddInitialState(...).AddState(...).WithContext(context).Build()`.
- Start execution with `await stateMachine.StartAsync()`.

**Examples**
- `TrafficLightExample` demonstrates sequential states and a terminal `OffState` after N cycles.
- `OrderProcessingExample` demonstrates conditional flow (payment success/failure, inventory checks) using context values.
- `CrawlerExample` demonstrates an iterative workflow with a URL queue, parsing and extraction, and a final summary.

**Testing**
Run unit tests (MSTest):

```powershell
cd tests\PureSM.Tests
dotnet test
```

**Contributing**
- Fork, implement a small focused change, add tests where applicable, and open a pull request.
- Keep changes targeted to one area per PR and follow the project coding style.

**Notes**
- The library targets multiple frameworks (net5.0, net6.0, net7.0, net8.0, net9.0). Conditional `ImplicitUsings` and small compatibility shims are in place to maintain net5.0 compatibility.
- Example projects live under `examples/` — their project names were recently updated to more descriptive names.

---

If you want, I can:
- Update `examples/README.md` to reflect the new example names and add quick-run snippets (I can do this next).
- Run all examples and collect sample outputs into a single `examples/demo-output.md` file.
