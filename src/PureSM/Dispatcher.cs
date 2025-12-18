using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Dispatches the execution of a state machine, managing state transitions and execution flow.
    /// While this can be used directly for advanced scenarios, most users should use StateMachineBuilder for creating state machines.
    /// </summary>
    public class Dispatcher
    {
        private readonly State _initialState;
        private readonly List<State> _lastStates = new List<State>();
        private readonly List<State> _states;
        private readonly List<IVariable> _variables;

        /// <summary>
        /// Initializes a new instance of the Dispatcher class.
        /// </summary>
        /// <param name="initialState">The initial state to start execution from.</param>
        /// <param name="states">The list of all states in the state machine.</param>
        /// <param name="variables">The list of variables used in the state machine.</param>
        public Dispatcher(State initialState, List<State> states, List<IVariable> variables)
        {
            _initialState = initialState ?? throw new ArgumentNullException(nameof(initialState));
            _states = states ?? throw new ArgumentNullException(nameof(states));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        /// <summary>
        /// Executes the state machine asynchronously, starting from the initial state and following transitions.
        /// </summary>
        /// <param name="context">The context to pass through the state machine execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public async Task DispatchAsync(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var nextTransitions = await _initialState.HandleAsync();
            if (_initialState.IsEndState)
                return;
            bool continueNextState = true;
            var nextStatesTasks = nextTransitions
                    .Select(async t => await t.Triggered(context, _initialState));
            var nextStates = (await Task.WhenAll(nextStatesTasks))
                            .Where(s => s != null)
                            .SelectMany(s => s!)
                            .Where(s => s != null);
            var nextTransitionsList = new List<(State state, Transition tran)>();
            while (continueNextState)
            { 
                if(!nextStates.Any())
                {
                    continueNextState = false;
                    break;
                }
                nextTransitionsList.Clear();
                foreach (var state in nextStates)
                { 
                    if (state != null)
                    {
                        if (state.IsEndState)
                            _lastStates.Add(state);
                        else
                        {
                            await state.HandleAsync();
                            nextTransitionsList.AddRange(state.Transitions.Select(t=>(state,t)));
                        }
                    }
                }
                var tks = nextTransitionsList
                    .Select(async t => await t.tran.Triggered(context, t.state));
                nextStates = (await Task.WhenAll(tks))
                            .Where(s => s != null)
                            .SelectMany(s => s!)
                            .Where(s => s != null);
            }
            foreach (var _lastState in _lastStates)
                await _lastState.HandleAsync();
        }
    }
}
