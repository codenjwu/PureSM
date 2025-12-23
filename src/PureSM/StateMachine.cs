using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Represents a state machine that can be executed with a given context.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Gets the unique identifier associated with this instance.
        /// </summary>
        public string Identifier { get; init; } = string.Empty;

        private readonly Dispatcher _dispatcher;
        private readonly Context _context;

        /// <summary>
        /// Initializes a new instance of the StateMachine class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that manages state transitions.</param>
        /// <param name="context">The context to pass to the state machine.</param>
        public StateMachine(Dispatcher dispatcher, Context context)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <summary>
        /// Initializes a new instance of the StateMachine class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that manages state transitions.</param>
        /// <param name="context">The context to pass to the state machine.</param>
        public StateMachine(Dispatcher dispatcher, Context context, string dentifier)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Identifier = dentifier;
        }

        /// <summary>
        /// Starts the state machine execution asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            await _dispatcher.DispatchAsync(_context);
        }

        /// <summary>
        /// Starts the state machine execution synchronously.
        /// </summary>
        public void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the context associated with this state machine.
        /// </summary>
        public Context Context => _context;
    }
}
