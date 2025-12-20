using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Fluent builder for constructing state machines with a convenient API.
    /// </summary>
    public class StateMachineBuilder
    {
        private State? _initialState;
        private State? _currentState;
        private readonly List<State> _states = new();
        private Context? _context;

        /// <summary>
        /// Sets the initial state for the state machine.
        /// </summary>
        /// <param name="state">The initial state.</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when state is null.</exception>
        public StateMachineBuilder AddInitialState(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state), "Initial state cannot be null.");

            _initialState = state;
            _currentState = state;
            _states.Add(state);
            return this;
        }

        /// <summary>
        /// Adds a state to the state machine and switches to configuring it.
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when state is null.</exception>
        public StateMachineBuilder AddState(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state), "State cannot be null.");

            _currentState = state;
            if (!_states.Contains(state))
                _states.Add(state);
            return this;
        }

        /// <summary>
        /// Adds a transition to the current state.
        /// </summary>
        /// <param name="condition">The condition function that determines if the transition should be triggered.</param>
        /// <param name="targetState">The target state for this transition.</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no current state is set.</exception>
        /// <exception cref="ArgumentNullException">Thrown when condition or targetState is null.</exception>
        public StateMachineBuilder AddTransition(Func<Context, State, Task<bool>> condition, State targetState)
        {
            if (_currentState == null)
                throw new InvalidOperationException("No current state set. Call AddInitialState or AddState first.");

            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");

            if (targetState == null)
                throw new ArgumentNullException(nameof(targetState), "Target state cannot be null.");

            var transition = new Transition(condition, new List<State> { targetState }, null);
            _currentState.AddTransition(transition);
            return this;
        }

        /// <summary>
        /// Adds a transition to the current state with multiple target states.
        /// </summary>
        /// <param name="condition">The condition function that determines if the transition should be triggered.</param>
        /// <param name="targetStates">The target states for this transition.</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no current state is set.</exception>
        /// <exception cref="ArgumentNullException">Thrown when condition or targetStates is null.</exception>
        public StateMachineBuilder AddTransition(Func<Context, State, Task<bool>> condition, params State[] targetStates)
        {
            if (_currentState == null)
                throw new InvalidOperationException("No current state set. Call AddInitialState or AddState first.");

            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");

            if (targetStates == null || targetStates.Length == 0)
                throw new ArgumentException("At least one target state must be provided.", nameof(targetStates));

            var transition = new Transition(condition, targetStates.ToList(), null);
            _currentState.AddTransition(transition);
            return this;
        }

        /// <summary>
        /// Sets the context for the state machine. If not set, a new Context will be created.
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public StateMachineBuilder WithContext(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null.");
            return this;
        }

        /// <summary>
        /// Builds the state machine from the configured states and transitions.
        /// </summary>
        /// <returns>A new StateMachine instance ready to be started.</returns>
        /// <exception cref="InvalidOperationException">Thrown when initial state is not set or states are not properly configured.</exception>
        public StateMachine Build()
        {
            if (_initialState == null)
                throw new InvalidOperationException("Initial state must be set before building. Call AddInitialState first.");

            if (_states.Count == 0)
                throw new InvalidOperationException("At least one state must be added before building.");

            var context = _context ?? new Context();
            var dispatcher = new Dispatcher(_initialState, _states);

            return new StateMachine(dispatcher, context);
        }

        /// <summary>
        /// Gets the initial state of the state machine being built.
        /// </summary>
        public State? InitialState => _initialState;

        /// <summary>
        /// Gets the current state being configured in the builder.
        /// </summary>
        /// <remarks>
        /// This represents the state that transitions will be added to when calling AddTransition.
        /// It is not the runtime current state of the state machine (which is managed by the Dispatcher).
        /// </remarks>
        public State? CurrentState => _currentState;
    }
}
