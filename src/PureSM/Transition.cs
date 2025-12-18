using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Represents a transition between states in a state machine.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Gets the variable associated with this transition.
        /// </summary>
        public IVariable? Variable { get; }

        /// <summary>
        /// Gets the condition function that determines if this transition should be triggered.
        /// </summary>
        public Func<Context, State, Task<bool>> Condition { get; }

        /// <summary>
        /// Gets the read-only list of target states for this transition.
        /// </summary>
        public IReadOnlyList<State> To { get; }

        private readonly List<State> _toMutable;

        /// <summary>
        /// Initializes a new instance of the Transition class.
        /// </summary>
        /// <param name="condition">The condition function to evaluate.</param>
        /// <param name="targetStates">The target states to transition to if the condition is true.</param>
        /// <param name="variable">Optional variable associated with this transition.</param>
        public Transition(Func<Context, State, Task<bool>> condition, List<State> targetStates, IVariable? variable)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _toMutable = targetStates ?? throw new ArgumentNullException(nameof(targetStates));
            To = _toMutable.AsReadOnly();
            Variable = variable;
        } 

        /// <summary>
        /// Evaluates the condition and returns the target states if the condition is met.
        /// </summary>
        /// <param name="context">The state machine context.</param>
        /// <param name="previousState">The current state.</param>
        /// <returns>The list of target states if the condition is true; otherwise null.</returns>
        public async Task<List<State>?> Triggered(Context context, State previousState)
        { 
            if( await Condition(context, previousState))
            {
                return _toMutable;
            }
            return null;
        } 
    }
}
