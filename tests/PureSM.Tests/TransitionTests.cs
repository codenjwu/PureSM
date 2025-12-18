using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureSM.Tests
{
    [TestClass]
    public class TransitionTests
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
        public async Task Triggered_ConditionReturnsTrue_ReturnsTargetStates()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            var targetStates = new List<State> { targetState };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, targetStates, null);

            // Act
            var result = await transition.Triggered(_context, _state);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(targetState, result[0]);
        }

        [TestMethod]
        public async Task Triggered_ConditionReturnsFalse_ReturnsNull()
        {
            // Arrange
            var targetState = new TestState(_context, true);
            var targetStates = new List<State> { targetState };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(false);
            var transition = new Transition(condition, targetStates, null);

            // Act
            var result = await transition.Triggered(_context, _state);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Constructor_WithNullCondition_ThrowsArgumentNullException()
        {
            // Arrange
            var targetStates = new List<State> { _state };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Transition(null, targetStates, null));
        }

        [TestMethod]
        public void Constructor_WithNullTargetStates_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Transition(condition, null, null));
        }

        [TestMethod]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange
            var targetStates = new List<State> { _state };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act
            var transition = new Transition(condition, targetStates, null);

            // Assert
            Assert.IsNotNull(transition);
            Assert.IsNotNull(transition.Condition);
            Assert.IsNotNull(transition.To);
            Assert.AreEqual(1, transition.To.Count);
        }

        [TestMethod]
        public void To_ReturnsReadOnlyListOfTargetStates()
        {
            // Arrange
            var targetState1 = new TestState(_context, false);
            var targetState2 = new TestState(_context, true);
            var targetStates = new List<State> { targetState1, targetState2 };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, targetStates, null);

            // Act
            var result = transition.To;

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(targetState1, result[0]);
            Assert.AreEqual(targetState2, result[1]);
        }

        [TestMethod]
        public void Variable_WithVariable_ReturnsVariable()
        {
            // Arrange
            var variable = new TestVariable();
            var targetStates = new List<State> { _state };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, targetStates, variable);

            // Act
            var result = transition.Variable;

            // Assert
            Assert.AreEqual(variable, result);
        }

        [TestMethod]
        public void Variable_WithoutVariable_ReturnsNull()
        {
            // Arrange
            var targetStates = new List<State> { _state };
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);
            var transition = new Transition(condition, targetStates, null);

            // Act
            var result = transition.Variable;

            // Assert
            Assert.IsNull(result);
        }

        private class TestState : State
        {
            public TestState(Context context, bool isEndState) 
                : base(context, isEndState)
            {
            }

            public override Task<State> Action() => Task.FromResult<State>(this);
            public override Task<State> Entry() => Task.FromResult<State>(this);
            public override Task<State> Exit() => Task.FromResult<State>(this);
        }

        private class TestVariable : IVariable
        {
            public string Name => "TestVariable";
            public void SetValue(object value) { }
            public object GetValue() => null;
        }
    }
}
