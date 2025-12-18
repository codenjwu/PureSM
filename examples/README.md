# PureSM Examples

This folder contains example projects demonstrating how to use the PureSM (Pure State Machine) library.

## Projects

### 1. SimpleExample
**Traffic Light State Machine**

A basic example demonstrating:
- Creating simple states (Red, Green, Yellow)
- Setting up transitions between states
- Using the StateMachineBuilder to construct a state machine
- Running the state machine with async/await

**To run:**
```bash
cd SimpleExample
dotnet run
```

**Output:**
```
=== Traffic Light State Machine Example ===

Starting traffic light sequence...

🔴 Red Light - STOP
   Waiting for 30 seconds...
   Transitioning to Green...

🟢 Green Light - GO
   Waiting for 25 seconds...
   Transitioning to Yellow...

🟡 Yellow Light - CAUTION
   Waiting for 5 seconds...
   Transitioning to Red...

=== State machine execution complete ===
```

### 2. AdvancedExample
**Order Processing State Machine**

A more complex example demonstrating:
- Using Context to store and pass data through state transitions
- Conditional transitions based on context data
- More realistic state machine with multiple states (Pending → Payment → Shipping → Delivered)
- Random conditions to simulate real-world scenarios (payment success/failure, stock availability)

**To run:**
```bash
cd AdvancedExample
dotnet run
```

**Sample Output:**
```
=== Order Processing State Machine Example ===

Processing order...

📋 Order State: PENDING
   Order ID: ORD-2025-001
   Validating order details...
   Order validated. Moving to payment processing...

💳 Order State: PAYMENT PROCESSING
   Processing payment of $150...
   ✓ Payment processed successfully!
   Moving to shipping...

📦 Order State: SHIPPING
   Packing 3 items...
   Generating shipping label...
   Items picked from warehouse...
   ✓ All items in stock and ready to ship!
   Handing off to delivery carrier...

✅ Order State: DELIVERED
   Order ORD-2025-001 has been delivered!
   Sending delivery confirmation email...
   Order processing complete!

=== Final Order State ===
Order ID: ORD-2025-001
Amount: $150
Quantity: 3
Payment Processed: True
Items in Stock: True
```

## Key Concepts

### Creating States
```csharp
public class MyState : State
{
    public MyState(Context context) : base(context, false) { }

    public override Task<State> Entry()
    {
        // Called when entering this state
        Console.WriteLine("Entering MyState");
        return Task.FromResult<State>(this);
    }

    public override Task<State> Action()
    {
        // Main action of the state
        Console.WriteLine("Performing action");
        return Task.FromResult<State>(this);
    }

    public override Task<State> Exit()
    {
        // Called when exiting this state
        Console.WriteLine("Exiting MyState");
        return Task.FromResult<State>(this);
    }
}
```

### Creating Transitions
```csharp
// Simple transition (always executes)
var transition = new Transition(
    async (ctx, state) => await Task.FromResult(true),
    new List<State> { nextState },
    null
);

// Conditional transition (executes based on context)
var conditionalTransition = new Transition(
    async (ctx, state) =>
    {
        var flag = ctx.GetItem("myFlag");
        return await Task.FromResult(flag != null && (bool)flag);
    },
    new List<State> { nextState },
    null
);
```

### Building a State Machine
```csharp
var stateMachine = new StateMachineBuilder()
    .AddInitialState(initialState)
    .AddState(state2)
    .AddState(state3)
    .AddTransition(condition1, state2)
    .AddTransition(condition2, state3)
    .WithContext(context)
    .Build();

// Run it
await stateMachine.StartAsync();
```

### Using Context
```csharp
var context = new Context();

// Store data
context.SetItem("key", "value");
context.SetItem("count", 42);

// Retrieve data
var value = context.GetItem("key");
var count = context.GetItem("count");

// Generic retrieval
var obj = context.GetItem<MyClass>("myObject");

// Check existence
if (context.ContainsKey("key"))
{
    // Process
}
```

## Framework Support

All examples support multiple .NET versions:
- .NET 5.0
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET 9.0

## Building All Examples

From the examples directory:
```bash
dotnet build
```

## Testing

To run tests for the library:
```bash
cd ../tests/PureSM.Tests
dotnet test
```
