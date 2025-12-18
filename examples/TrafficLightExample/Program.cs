using System;
using System.Threading.Tasks;
using PureSM;

namespace TrafficLightExample
{
    // Traffic Light example - a simple state machine
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Traffic Light State Machine Example ===\n");

            // Create the context - track cycles
            var context = new Context();
            context.SetItem("cycle_count", (object)0);
            context.SetItem("max_cycles", (object)3);

            // Create states
            var redState = new RedLight(context);
            var greenState = new GreenLight(context);
            var yellowState = new YellowLight(context);
            var offState = new OffState(context);

            // Condition: Check if we've completed max cycles
            Func<Context, State, Task<bool>> shouldGoToOff = async (ctx, state) =>
            {
                int cycleCount = (int)ctx.GetItem<object>("cycle_count");
                int maxCycles = (int)ctx.GetItem<object>("max_cycles");
                return await Task.FromResult(cycleCount >= maxCycles);
            };

            // Create transitions
            // Red -> Green (after red light period)
            var redToGreen = new Transition(
                async (ctx, state) => await Task.FromResult(true),
                new System.Collections.Generic.List<State> { greenState },
                null
            );

            // Green -> Yellow (after green light period)
            var greenToYellow = new Transition(
                async (ctx, state) => await Task.FromResult(true),
                new System.Collections.Generic.List<State> { yellowState },
                null
            );

            // Yellow -> Red or Off (if cycle count reached)
            var yellowToRed = new Transition(
                async (ctx, state) => await shouldGoToOff(ctx, state),
                new System.Collections.Generic.List<State> { offState },
                null
            );

            var yellowToRedContinue = new Transition(
                async (ctx, state) => !(await shouldGoToOff(ctx, state)),
                new System.Collections.Generic.List<State> { redState },
                null
            );

            // Add transitions to states
            redState.AddTransition(redToGreen);
            greenState.AddTransition(greenToYellow);
            yellowState.AddTransition(yellowToRed);
            yellowState.AddTransition(yellowToRedContinue);

            // Build the state machine
            var stateMachine = new StateMachineBuilder()
                .AddInitialState(redState)
                .AddState(greenState)
                .AddState(yellowState)
                .AddState(offState)
                .WithContext(context)
                .Build();

            // Run the state machine
            Console.WriteLine("Starting traffic light sequence...\n");
            await stateMachine.StartAsync();

            Console.WriteLine("\n=== State machine execution complete ===");
        }
    }

    // Red Light State
    public class RedLight : State
    {
        public RedLight(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("🔴 Red Light - STOP");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Waiting for 30 seconds...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   Transitioning to Green...");
            return Task.FromResult<State>(this);
        }
    }

    // Green Light State
    public class GreenLight : State
    {
        public GreenLight(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n🟢 Green Light - GO");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Waiting for 25 seconds...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   Transitioning to Yellow...");
            return Task.FromResult<State>(this);
        }
    }

    // Yellow Light State
    public class YellowLight : State
    {
        public YellowLight(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n🟡 Yellow Light - CAUTION");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Waiting for 5 seconds...");
            
            // Increment cycle count
            int cycleCount = (int)Context.GetItem<object>("cycle_count");
            Context.SetItem("cycle_count", (object)(cycleCount + 1));
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            int cycleCount = (int)Context.GetItem<object>("cycle_count");
            int maxCycles = (int)Context.GetItem<object>("max_cycles");
            
            if (cycleCount >= maxCycles)
            {
                Console.WriteLine("   Transitioning to Off...");
            }
            else
            {
                Console.WriteLine("   Transitioning to Red...");
            }
            
            return Task.FromResult<State>(this);
        }
    }

    // Off State - Final state
    public class OffState : State
    {
        public OffState(Context context) : base(context, isEndState: true) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n⚫ Traffic Light - OFF");
            int cycles = (int)Context.GetItem<object>("cycle_count");
            Console.WriteLine($"   Completed {cycles} full cycles");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   All lights off - traffic light shutdown complete");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            return Task.FromResult<State>(this);
        }
    }
}
