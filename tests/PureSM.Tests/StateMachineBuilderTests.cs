using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureSM.Tests
{
    [TestClass]
    public class StateMachineBuilderTests
    {
        private Context _context;

        [TestInitialize]
        public void Setup()
        {
            _context = new Context();
        }

        [TestMethod]
        public void AddInitialState_WithValidState_SetsInitialState()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state = new TestState(_context, false);

            // Act
            var result = builder.AddInitialState(state);

            // Assert
            Assert.AreEqual(builder, result); // Check fluent API
            var stateMachine = builder.Build();
            Assert.IsNotNull(stateMachine);
        }

        [TestMethod]
        public void AddInitialState_WithNullState_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new StateMachineBuilder();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                builder.AddInitialState(null));
        }

        [TestMethod]
        public void AddState_WithValidState_AddsState()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var initialState = new TestState(_context, false);
            var newState = new TestState(_context, false);
            builder.AddInitialState(initialState);

            // Act
            var result = builder.AddState(newState);

            // Assert
            Assert.AreEqual(builder, result); // Check fluent API
        }

        [TestMethod]
        public void AddState_WithNullState_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var initialState = new TestState(_context, false);
            builder.AddInitialState(initialState);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                builder.AddState(null));
        }

        [TestMethod]
        public void AddState_WithDuplicateState_DoesNotDuplicateInStateList()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var initialState = new TestState(_context, false);
            builder.AddInitialState(initialState);
            builder.AddState(initialState);

            // Act
            var stateMachine = builder.Build();

            // Assert
            Assert.IsNotNull(stateMachine);
        }

        [TestMethod]
        public void AddTransition_WithValidParameters_AddsTransition()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state1 = new TestState(_context, false);
            var state2 = new TestState(_context, true);
            builder.AddInitialState(state1);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act
            var result = builder.AddTransition(condition, state2);

            // Assert
            Assert.AreEqual(builder, result);
        }

        [TestMethod]
        public void AddTransition_WithoutCurrentState_ThrowsInvalidOperationException()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state = new TestState(_context, true);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.AddTransition(condition, state));
        }

        [TestMethod]
        public void AddTransition_WithNullCondition_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state1 = new TestState(_context, false);
            var state2 = new TestState(_context, true);
            builder.AddInitialState(state1);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                builder.AddTransition(null, state2));
        }

        [TestMethod]
        public void AddTransition_WithNullTargetState_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state1 = new TestState(_context, false);
            builder.AddInitialState(state1);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                builder.AddTransition(condition, (State)null));
        }

        [TestMethod]
        public void AddTransition_WithMultipleTargetStates_AddsTransitionWithMultipleTargets()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state1 = new TestState(_context, false);
            var state2 = new TestState(_context, false);
            var state3 = new TestState(_context, true);
            builder.AddInitialState(state1);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act
            var result = builder.AddTransition(condition, state2, state3);

            // Assert
            Assert.AreEqual(builder, result);
        }

        [TestMethod]
        public void AddTransition_WithNoTargetStates_ThrowsArgumentException()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var state1 = new TestState(_context, false);
            builder.AddInitialState(state1);
            Func<Context, State, Task<bool>> condition = async (ctx, s) => await Task.FromResult(true);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                builder.AddTransition(condition));
        }

        [TestMethod]
        public void WithContext_WithValidContext_SetsContext()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var customContext = new Context();

            // Act
            var result = builder.WithContext(customContext);

            // Assert
            Assert.AreEqual(builder, result);
        }

        [TestMethod]
        public void WithContext_WithNullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new StateMachineBuilder();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                builder.WithContext(null));
        }

        [TestMethod]
        public void Build_WithInitialState_BuildsStateMachine()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var initialState = new TestState(_context, true);
            builder.AddInitialState(initialState);

            // Act
            var stateMachine = builder.Build();

            // Assert
            Assert.IsNotNull(stateMachine);
        }

        [TestMethod]
        public void Build_WithoutInitialState_ThrowsInvalidOperationException()
        {
            // Arrange
            var builder = new StateMachineBuilder();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.Build());
        }

        [TestMethod]
        public void Build_WithoutStates_ThrowsInvalidOperationException()
        {
            // Arrange
            var builder = new StateMachineBuilder();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.Build());
        }

        [TestMethod]
        public void Build_WithCustomContext_UsesProvidedContext()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var customContext = new Context();
            customContext.SetItem("test", "value");
            var initialState = new TestState(customContext, true);
            builder.AddInitialState(initialState);
            builder.WithContext(customContext);

            // Act
            var stateMachine = builder.Build();

            // Assert
            Assert.AreEqual(customContext, stateMachine.Context);
        }

        [TestMethod]
        public void Build_WithoutProvidingContext_CreatesNewContext()
        {
            // Arrange
            var builder = new StateMachineBuilder();
            var initialState = new TestState(new Context(), true);
            builder.AddInitialState(initialState);

            // Act
            var stateMachine = builder.Build();

            // Assert
            Assert.IsNotNull(stateMachine.Context);
        }

        [TestMethod]
        public void FluentAPI_ComplexScenario_BuildsComplexStateMachine()
        {
            // Arrange
            var context = new Context();
            var builder = new StateMachineBuilder();
            var state1 = new TestState(context, false);
            var state2 = new TestState(context, false);
            var state3 = new TestState(context, true);

            Func<Context, State, Task<bool>> condition1 = async (ctx, s) => await Task.FromResult(true);
            Func<Context, State, Task<bool>> condition2 = async (ctx, s) => await Task.FromResult(true);

            // Act
            var stateMachine = builder
                .AddInitialState(state1)
                .AddTransition(condition1, state2)
                .AddState(state2)
                .AddTransition(condition2, state3)
                .AddState(state3)
                .WithContext(context)
                .Build();

            // Assert
            Assert.IsNotNull(stateMachine);
            Assert.AreEqual(context, stateMachine.Context);
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
    }
}
