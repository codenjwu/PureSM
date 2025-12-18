using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureSM.Tests
{
    [TestClass]
    public class StateTests
    {
        private Context _context;
        private TestState _state;

        [TestInitialize]
        public void Setup()
        {
            _context = new Context();
            _state = new TestState(_context, false);
        }

        [TestMethod]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act & Assert
            Assert.IsNotNull(_state);
            Assert.IsFalse(_state.IsEndState);
        }

        [TestMethod]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new TestState(null, false));
        }

        [TestMethod]
        public void Constructor_AsEndState_SetsIsEndStateTrue()
        {
            // Arrange & Act
            var endState = new TestState(_context, true);

            // Assert
            Assert.IsTrue(endState.IsEndState);
        }

        [TestMethod]
        public void AddTransition_WithValidTransition_AddsTransition()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, new List<State> { targetState }, null);

            // Act
            _state.AddTransition(transition);

            // Assert
            Assert.AreEqual(1, _state.Transitions.Count);
            Assert.AreEqual(transition, _state.Transitions[0]);
        }

        [TestMethod]
        public void AddTransition_WithNullTransition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _state.AddTransition(null));
        }

        [TestMethod]
        public void AddTransition_MultipleTransitions_AddsAllTransitions()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            Func<Context, State, Task<bool>> condition1 = async (ctx, s) => await Task.FromResult(true);
            Func<Context, State, Task<bool>> condition2 = async (ctx, s) => await Task.FromResult(false);
            var transition1 = new Transition(condition1, new List<State> { targetState }, null);
            var transition2 = new Transition(condition2, new List<State> { targetState }, null);

            // Act
            _state.AddTransition(transition1);
            _state.AddTransition(transition2);

            // Assert
            Assert.AreEqual(2, _state.Transitions.Count);
        }

        [TestMethod]
        public void Transitions_ReturnsReadOnlyList()
        {
            // Act
            var transitions = _state.Transitions;

            // Assert
            Assert.IsNotNull(transitions);
            Assert.AreEqual(0, transitions.Count);
        }

        [TestMethod]
        public async Task HandleAsync_CallsEntryActionAndExit()
        {
            // Arrange
            var state = new TrackingState(_context, false);

            // Act
            await state.HandleAsync();

            // Assert
            Assert.IsTrue(state.EntryWasCalled);
            Assert.IsTrue(state.ActionWasCalled);
            Assert.IsTrue(state.ExitWasCalled);
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsTransitions()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, new List<State> { targetState }, null);
            _state.AddTransition(transition);

            // Act
            var result = await _state.HandleAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(transition, result.First());
        }

        [TestMethod]
        public void Constructor_WithInitialTransitions_AddsTransitions()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, new List<State> { targetState }, null);

            // Act
            var state = new TestState(_context, false, transition);

            // Assert
            Assert.AreEqual(1, state.Transitions.Count);
            Assert.AreEqual(transition, state.Transitions[0]);
        }

        private class TestState : State
        {
            public TestState(Context context, bool isEndState)
                : base(context, isEndState)
            {
            }

            public TestState(Context context, bool isEndState, params Transition[] transitions)
                : base(context, isEndState, transitions)
            {
            }

            public override Task<State> Action() => Task.FromResult<State>(this);
            public override Task<State> Entry() => Task.FromResult<State>(this);
            public override Task<State> Exit() => Task.FromResult<State>(this);
        }

        private class TrackingState : State
        {
            public bool EntryWasCalled { get; private set; }
            public bool ActionWasCalled { get; private set; }
            public bool ExitWasCalled { get; private set; }

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
