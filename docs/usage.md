# Usage Guide

This section shows the typical steps to use PureSM in your code.

1. Create a `Context` and populate initial data
```csharp
var context = new Context();
context.SetItem("userId", (object)"user-123");
```

2. Implement `State` subclasses
- Override `Entry()`, `Action()`, and `Exit()` returning `Task<State>`.
- Use `Context` (protected property of `State`) to read/write shared data.

```csharp
public class MyState : State
{
    public MyState(Context ctx) : base(ctx, false) { }

    public override Task<State> Entry()
    {
        Console.WriteLine("Entering MyState");
        return Task.FromResult<State>(this);
    }

    public override Task<State> Action()
    {
        // core behavior
        return Task.FromResult<State>(this);
    }

    public override Task<State> Exit()
    {
        Console.WriteLine("Exiting MyState");
        return Task.FromResult<State>(this);
    }
}
```

3. Create `Transition` objects
- Provide an async condition function and a list of target `State` instances.
```csharp
var t = new Transition(async (ctx, s) => await Task.FromResult(true), new List<State>{ nextState }, null);
```

4. Attach transitions to states
```csharp
state.AddTransition(t);
```

5. Build the state machine
```csharp
var sm = new StateMachineBuilder()
    .AddInitialState(startState)
    .AddState(otherState)
    .WithContext(context)
    .Build();

await sm.StartAsync();
```

Notes
- Use `isEndState` (the second `State` constructor parameter) to mark terminal states where the machine should stop.
- Conditions can inspect and mutate the `Context` to make routing decisions.
- The library is Task-based; replace `Task.FromResult` with true async I/O when integrating network or file operations.