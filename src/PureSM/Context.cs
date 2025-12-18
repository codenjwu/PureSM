using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureSM
{
    /// <summary>
    /// Represents the context for a state machine execution, storing shared data between states.
    /// This is a thread-safe container for passing data through state machine transitions.
    /// </summary>
    public class Context
    {
        private readonly Dictionary<string, object?> _items = new Dictionary<string, object?>();

        /// <summary>
        /// Gets the items dictionary for this context. Use SetItem/GetItem for safe access.
        /// </summary>
        public IReadOnlyDictionary<string, object?> Items => new System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>(_items);

        /// <summary>
        /// Sets a value in the context.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        public void SetItem(string key, object? value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            _items[key] = value;
        }

        /// <summary>
        /// Gets a value from the context.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value associated with the key, or null if not found.</returns>
        public object? GetItem(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return _items.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Checks if a key exists in the context.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists; otherwise false.</returns>
        public bool ContainsKey(string key)
        {
            return key != null && _items.ContainsKey(key);
        }

        /// <summary>
        /// Gets a typed value from the context.
        /// </summary>
        /// <typeparam name="T">The expected type of the value.</typeparam>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value cast to type T, or default if not found or wrong type.</returns>
        public T? GetItem<T>(string key) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return GetItem(key) as T;
        }
    }
}
