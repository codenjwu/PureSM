using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PureSM;

namespace OrderProcessingExample
{
    // Order Processing State Machine - Advanced example with context data
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Order Processing State Machine Example ===\n");

            // Create context and store order data
            var context = new Context();
            context.SetItem("orderId", "ORD-2025-001");
            context.SetItem("amount", 150.00);
            context.SetItem("quantity", 3);
            context.SetItem("paymentProcessed", false);
            context.SetItem("itemsInStock", true);

            // Create states
            var pendingState = new PendingState(context);
            var paymentState = new PaymentProcessingState(context);
            var shippingState = new ShippingState(context);
            var deliveredState = new DeliveredState(context);

            // Create transitions with conditional logic
            
            // Pending -> Payment Processing (always proceed)
            var pendingToPayment = new Transition(
                async (ctx, state) => await Task.FromResult(true),
                new List<State> { paymentState },
                null
            );

            // Payment -> Shipping (if payment successful)
            var paymentToShipping = new Transition(
                async (ctx, state) =>
                {
                    var processed = ctx.GetItem("paymentProcessed");
                    return await Task.FromResult(processed != null && (bool)processed);
                },
                new List<State> { shippingState },
                null
            );

            // Shipping -> Delivered (if items are in stock)
            var shippingToDelivered = new Transition(
                async (ctx, state) =>
                {
                    var inStock = ctx.GetItem("itemsInStock");
                    return await Task.FromResult(inStock != null && (bool)inStock);
                },
                new List<State> { deliveredState },
                null
            );

            // Add transitions
            pendingState.AddTransition(pendingToPayment);
            paymentState.AddTransition(paymentToShipping);
            shippingState.AddTransition(shippingToDelivered);

            // Build state machine
            var stateMachine = new StateMachineBuilder()
                .AddInitialState(pendingState)
                .AddState(paymentState)
                .AddState(shippingState)
                .AddState(deliveredState)
                .WithContext(context)
                .Build();

            // Run the state machine
            Console.WriteLine("Processing order...\n");
            await stateMachine.StartAsync();

            // Display final context state
            Console.WriteLine("\n=== Final Order State ===");
            Console.WriteLine($"Order ID: {context.GetItem("orderId")}");
            Console.WriteLine($"Amount: ${context.GetItem("amount")}");
            Console.WriteLine($"Quantity: {context.GetItem("quantity")}");
            Console.WriteLine($"Payment Processed: {context.GetItem("paymentProcessed")}");
            Console.WriteLine($"Items in Stock: {context.GetItem("itemsInStock")}");
        }
    }

    // Pending State - Initial state
    public class PendingState : State
    {
        public PendingState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            var orderId = Context.GetItem("orderId");
            Console.WriteLine($"📋 Order State: PENDING");
            Console.WriteLine($"   Order ID: {orderId}");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Validating order details...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   Order validated. Moving to payment processing...\n");
            return Task.FromResult<State>(this);
        }
    }

    // Payment Processing State
    public class PaymentProcessingState : State
    {
        public PaymentProcessingState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("💳 Order State: PAYMENT PROCESSING");
            var amount = Context.GetItem("amount");
            Console.WriteLine($"   Processing payment of ${amount}...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            // Simulate payment processing
            var random = new Random();
            bool success = random.Next(0, 2) == 1;
            Context.SetItem("paymentProcessed", success);
            
            if (success)
            {
                Console.WriteLine("   ✓ Payment processed successfully!");
            }
            else
            {
                Console.WriteLine("   ✗ Payment failed!");
            }
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            if ((bool)Context.GetItem("paymentProcessed")!)
            {
                Console.WriteLine("   Moving to shipping...\n");
            }
            else
            {
                Console.WriteLine("   Order cancelled due to payment failure.\n");
            }
            return Task.FromResult<State>(this);
        }
    }

    // Shipping State
    public class ShippingState : State
    {
        public ShippingState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("📦 Order State: SHIPPING");
            var quantity = Context.GetItem("quantity");
            Console.WriteLine($"   Packing {quantity} items...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Generating shipping label...");
            Console.WriteLine("   Items picked from warehouse...");
            var inStock = new Random().Next(0, 2) == 1;
            Context.SetItem("itemsInStock", inStock);
            
            if (inStock)
            {
                Console.WriteLine("   ✓ All items in stock and ready to ship!");
            }
            else
            {
                Console.WriteLine("   ✗ Some items out of stock!");
            }
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            if ((bool)Context.GetItem("itemsInStock")!)
            {
                Console.WriteLine("   Handing off to delivery carrier...\n");
            }
            else
            {
                Console.WriteLine("   Order on hold pending stock.\n");
            }
            return Task.FromResult<State>(this);
        }
    }

    // Delivered State - Final state
    public class DeliveredState : State
    {
        public DeliveredState(Context context) : base(context, true) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("✅ Order State: DELIVERED");
            var orderId = Context.GetItem("orderId");
            Console.WriteLine($"   Order {orderId} has been delivered!");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("   Sending delivery confirmation email...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   Order processing complete!");
            return Task.FromResult<State>(this);
        }
    }
}
