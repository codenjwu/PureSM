using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureSM.Tests
{
    [TestClass]
    public class StateMachineTests
    {
        private Context _context;

        [TestInitialize]
        public void Setup()
        {
            _context = new Context();
        }

        [TestMethod]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange
            var dispatcher = CreateSimpleDispatcher();

            // Act
            var stateMachine = new StateMachine(dispatcher, _context);

            // Assert
            Assert.IsNotNull(stateMachine);
            Assert.AreEqual(_context, stateMachine.Context);
        }

        [TestMethod]
        public void Constructor_WithNullDispatcher_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StateMachine(null, _context));
        }

        [TestMethod]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var dispatcher = CreateSimpleDispatcher();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StateMachine(dispatcher, null));
        }

        [TestMethod]
        public async Task StartAsync_ExecutesDispatcher()
        {
            // Arrange
            var initialState = new TrackingState(_context, true);
            var dispatcher = new Dispatcher(initialState, new List<State> { initialState });
            var stateMachine = new StateMachine(dispatcher, _context);

            // Act
            await stateMachine.StartAsync();

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
        }

        [TestMethod]
        public void Start_ExecutesDispatcherSynchronously()
        {
            // Arrange
            var initialState = new TrackingState(_context, true);
            var dispatcher = new Dispatcher(initialState, new List<State> { initialState });
            var stateMachine = new StateMachine(dispatcher, _context);

            // Act
            stateMachine.Start();

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
        }

        [TestMethod]
        public async Task StartAsync_WithTransitioningStates_ExecutesAllStates()
        {
            // Arrange
            var state1 = new TrackingState(_context, false);
            var state2 = new TrackingState(_context, true);
            var states = new List<State> { state1, state2 };

            Func<Context, State, Task<bool>> alwaysTrue = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(alwaysTrue, new List<State> { state2 }, null);
            state1.AddTransition(transition);

            var dispatcher = new Dispatcher(state1, states);
            var stateMachine = new StateMachine(dispatcher, _context);

            // Act
            await stateMachine.StartAsync();

            // Assert
            Assert.IsTrue(state1.EntryWasCalled);
            Assert.IsTrue(state2.EntryWasCalled);
        }

        [TestMethod]
        public void Context_ReturnsProvidedContext()
        {
            // Arrange
            var customContext = new Context();
            customContext.SetItem("key", "value");
            var dispatcher = CreateSimpleDispatcher();
            var stateMachine = new StateMachine(dispatcher, customContext);

            // Act
            var context = stateMachine.Context;

            // Assert
            Assert.AreEqual(customContext, context);
            Assert.AreEqual("value", context.GetItem("key"));
        }

        [TestMethod]
        public async Task StartAsync_WithContextData_PreservesData()
        {
            // Arrange
            var key = "testData";
            var value = "testValue";
            _context.SetItem(key, value);

            var initialState = new TrackingState(_context, true);
            var dispatcher = new Dispatcher(initialState, new List<State> { initialState });
            var stateMachine = new StateMachine(dispatcher, _context);

            // Act
            await stateMachine.StartAsync();

            // Assert
            Assert.AreEqual(value, _context.GetItem(key));
        }

        [TestMethod]
        public async Task StartAsync_MultipleExecutions_CanBeCalledMultipleTimes()
        {
            // Arrange
            var state = new TrackingState(_context, true);
            var dispatcher = new Dispatcher(state, new List<State> { state });
            var stateMachine = new StateMachine(dispatcher, _context);

            // Act
            await stateMachine.StartAsync();
            await stateMachine.StartAsync();

            // Assert
            Assert.IsTrue(state.EntryWasCalled);
        }

        private Dispatcher CreateSimpleDispatcher()
        {
            var state = new SimpleState(_context, true);
            return new Dispatcher(state, new List<State> { state });
        }

        private class SimpleState : State
        {
            public SimpleState(Context context, bool isEndState)
                : base(context, isEndState)
            {
            }

            public override Task<State> Action() => Task.FromResult<State>(this);
            public override Task<State> Entry() => Task.FromResult<State>(this);
            public override Task<State> Exit() => Task.FromResult<State>(this);
        }

        private class TrackingState : State
        {
            public bool EntryWasCalled { get; set; }
            public bool ActionWasCalled { get; set; }
            public bool ExitWasCalled { get; set; }

            public TrackingState(Context context, bool isEndState)
                : base(context, isEndState)
            {
            }

            public override Task<State> Action()
            {
                ActionWasCalled = true;
                return Task.FromResult<State>(this);
            }

            public override Task<State> Entry()
            {
                EntryWasCalled = true;
                return Task.FromResult<State>(this);
            }

            public override Task<State> Exit()
            {
                ExitWasCalled = true;
                return Task.FromResult<State>(this);
            }
        }
    }
}
