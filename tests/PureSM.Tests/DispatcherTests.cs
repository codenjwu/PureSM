using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureSM.Tests
{
    [TestClass]
    public class DispatcherTests
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
            var initialState = new SimpleTestState(_context, false);
            var states = new List<State> { initialState };

            // Act
            var dispatcher = new Dispatcher(initialState, states);

            // Assert
            Assert.IsNotNull(dispatcher);
        }

        [TestMethod]
        public void Constructor_WithNullInitialState_ThrowsArgumentNullException()
        {
            // Arrange
            var states = new List<State>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Dispatcher(null, states));
        }

        [TestMethod]
        public void Constructor_WithNullStates_ThrowsArgumentNullException()
        {
            // Arrange
            var initialState = new SimpleTestState(_context, false);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Dispatcher(initialState, null));
        }


        [TestMethod]
        public async Task DispatchAsync_WithNullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var initialState = new SimpleTestState(_context, false);
            var states = new List<State> { initialState };
            var dispatcher = new Dispatcher(initialState, states);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                dispatcher.DispatchAsync(null));
        }

        [TestMethod]
        public async Task DispatchAsync_WithEndStateAsInitialState_ExecutesAndReturns()
        {
            // Arrange
            var initialState = new TrackingTestState(_context, true);
            var states = new List<State> { initialState };
            var dispatcher = new Dispatcher(initialState, states);

            // Act
            await dispatcher.DispatchAsync(_context);

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
        }

        [TestMethod]
        public async Task DispatchAsync_WithSimpleTransition_ExecutesStatesInOrder()
        {
            // Arrange
            var initialState = new TrackingTestState(_context, false);
            var endState = new TrackingTestState(_context, true);
            var states = new List<State> { initialState, endState };

            // Add transition from initial to end state
            Func<Context, State, Task<bool>> alwaysTrue = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(alwaysTrue, new List<State> { endState }, null);
            initialState.AddTransition(transition);

            var dispatcher = new Dispatcher(initialState, states);

            // Act
            await dispatcher.DispatchAsync(_context);

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
            Assert.IsTrue(endState.EntryWasCalled);
        }

        [TestMethod]
        public async Task DispatchAsync_WithNoValidTransitions_ExecutesInitialStateOnly()
        {
            // Arrange
            var initialState = new TrackingTestState(_context, false);
            var states = new List<State> { initialState };

            var dispatcher = new Dispatcher(initialState, states);

            // Act
            await dispatcher.DispatchAsync(_context);

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
        }

        [TestMethod]
        public async Task DispatchAsync_WithMultipleTransitions_FollowsCorrectPath()
        {
            // Arrange
            var initialState = new TrackingTestState(_context, false);
            var intermediateState = new TrackingTestState(_context, false);
            var endState = new TrackingTestState(_context, true);

            var states = new List<State> { initialState, intermediateState, endState };

            // Create transitions
            Func<Context, State, Task<bool>> alwaysTrue = async (ctx, s) => await Task.FromResult(true);
            var transition1 = new Transition(alwaysTrue, new List<State> { intermediateState }, null);
            var transition2 = new Transition(alwaysTrue, new List<State> { endState }, null);

            initialState.AddTransition(transition1);
            intermediateState.AddTransition(transition2);

            var dispatcher = new Dispatcher(initialState, states);

            // Act
            await dispatcher.DispatchAsync(_context);

            // Assert
            Assert.IsTrue(initialState.EntryWasCalled);
            Assert.IsTrue(intermediateState.EntryWasCalled);
            Assert.IsTrue(endState.EntryWasCalled);
        }

        [TestMethod]
        public async Task DispatchAsync_WithContextData_PassesDataThroughStates()
        {
            // Arrange
            var testData = "testData";
            var dataKey = "data";
            _context.SetItem(dataKey, testData);

            var initialState = new SimpleTestState(_context, false);
            var endState = new SimpleTestState(_context, true);
            var states = new List<State> { initialState, endState };

            Func<Context, State, Task<bool>> alwaysTrue = async (ctx, s) =>
            {
                var value = ctx.GetItem(dataKey);
                return await Task.FromResult(value != null);
            };

            var transition = new Transition(alwaysTrue, new List<State> { endState }, null);
            initialState.AddTransition(transition);

            var dispatcher = new Dispatcher(initialState, states);

            // Act
            await dispatcher.DispatchAsync(_context);

            // Assert
            Assert.AreEqual(testData, _context.GetItem(dataKey));
        }

        private class SimpleTestState : State
        {
            public SimpleTestState(Context context, bool isEndState)
                : base(context, isEndState)
            {
            }

            public override Task<State> Action() => Task.FromResult<State>(this);
            public override Task<State> Entry() => Task.FromResult<State>(this);
            public override Task<State> Exit() => Task.FromResult<State>(this);
        }

        private class TrackingTestState : State
        {
            public bool EntryWasCalled { get; set; }
            public bool ActionWasCalled { get; set; }
            public bool ExitWasCalled { get; set; }

            public TrackingTestState(Context context, bool isEndState)
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
