# API Reference (quick)

This is a short reference to the primary types in PureSM.

`State` (abstract)
- Purpose: base class for states.
- Key members:
  - `protected Context Context { get; }` — access the shared context
  - `public IReadOnlyList<Transition> Transitions` — registered transitions
  - `protected State(Context context, bool isEndState, params Transition[] transitions)` — constructor
  - `public void AddTransition(Transition t)` — add a transition
  - `public abstract Task<State> Entry()` — called when entering the state
  - `public abstract Task<State> Action()` — main action
  - `public abstract Task<State> Exit()` — called before leaving the state

`Transition`
- Purpose: describes conditional routing from a source state to one or more target states.
- Typical constructor: `new Transition(Func<Context, State, Task<bool>> condition, List<State> targets, object? meta)`
- The state machine evaluates transitions (in insertion order) and takes the first condition returning `true`.

`Context`
- Purpose: simple key/value bag passed through the state machine.
- Key methods:
  - `SetItem(string key, object? value)`
  - `GetItem<T>(string key)` — note: `T` generally should be a reference type in this repository's design; examples use `object` and cast for value types.
  - `ContainsKey(string key)`

`StateMachineBuilder`
- Fluent API to register initial and additional states and to attach the `Context`.
- `Build()` returns a `StateMachine` ready to run.

`StateMachine`
- Use `StartAsync()` to begin execution from the initial state.
- The machine runs states in sequence until a terminal (`isEndState`) state is reached or no transitions are taken.

Notes
- The library is intentionally small; consult `src/PureSM` source files for deeper details and available types.