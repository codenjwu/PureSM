using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Abstract base class representing a state in a state machine.
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Gets the unique identifier associated with this instance.
        /// </summary>
        public string Identifier { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the variable associated with this state.
        /// </summary>
        public IVariable? Variable { get; private set; }

        /// <summary>
        /// Gets the context passed through the state machine.
        /// </summary>
        protected Context Context { get; }

        private readonly List<Transition> _transitions = new List<Transition>();

        /// <summary>
        /// Gets the read-only list of transitions from this state.
        /// </summary>
        public IReadOnlyList<Transition> Transitions => _transitions.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the State class.
        /// </summary>
        /// <param name="context">The context passed through the state machine.</param>
        /// <param name="isEndState">Indicates whether this is an end state.</param>
        /// <param name="transitions">Optional initial transitions for this state.</param>
        protected State(Context context, bool isEndState, params Transition[] transitions)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            IsEndState = isEndState;
            if(transitions != null)
                _transitions.AddRange(transitions);
        }

        /// <summary>
        /// Initializes a new instance of the State class.
        /// </summary>
        /// <param name="context">The context passed through the state machine.</param>
        /// <param name="isEndState">Indicates whether this is an end state.</param>
        /// <param name="transitions">Optional initial transitions for this state.</param>
        protected State(Context context, bool isEndState, string identifier, params Transition[] transitions)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            IsEndState = isEndState;
            Identifier = identifier;
            if (transitions != null)
                _transitions.AddRange(transitions);
        }

        /// <summary>
        /// Adds a transition to this state.
        /// </summary>
        /// <param name="transition">The transition to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when transition is null.</exception>
        public void AddTransition(Transition transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));
            _transitions.Add(transition);
        }

        /// <summary>
        /// Performs the main action of this state. Called after Entry().
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task<State?> Action();

        /// <summary>
        /// Called when entering this state, before Action().
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task<State?> Entry();

        /// <summary>
        /// Called when exiting this state, after Action().
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task<State?> Exit();

        /// <summary>
        /// Handles the execution of this state by calling Entry, Action, and Exit in sequence.
        /// </summary>
        /// <returns>The list of transitions available from this state.</returns>
        public async Task<IEnumerable<Transition>> HandleAsync()
        {
            await Entry();
            await Action();
            await Exit();
            return Transitions;
        }

        /// <summary>
        /// Gets a value indicating whether this is an end state.
        /// </summary>
        public bool IsEndState { get; }
    }
}
