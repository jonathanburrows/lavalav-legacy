using System;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    ///     Provides a consistent way to parse command line arguments.
    /// </summary>
    /// <remarks>
    ///     Command line arguments should have a space between the key and value.
    /// </remarks>
    public class ArgumentParser
    {
        /// <summary>
        ///     Checks if a given flag is in the list of arguments.
        /// </summary>
        /// <param name="args">The list of arguments which could potentially contain the key.</param>
        /// <param name="key">The flag which is being searched for.</param>
        /// <returns>
        ///     True if the flag is in the argument list, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="flag"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="flag"/> does not begin with --</exception>
        public bool HasFlag(string[] args, string flag)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (flag == null)
            {
                throw new ArgumentNullException(nameof(flag));
            }
            if (!flag.StartsWith("--"))
            {
                throw new InvalidOperationException($"Flag {flag} does not start with --");
            }

            return GetIndexOfKey(args, flag) > -1;
        }

        /// <summary>
        ///     Returns a value from a list of arguments if found, or the default value if not.
        /// </summary>
        /// <typeparam name="T">The type which the argument will be cast to.</typeparam>
        /// <param name="args">The list of arguments which may contain the key/value</param>
        /// <param name="key">The identifier of the argument.</param>
        /// <returns>
        ///     If the argument list has the key, the next value, otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="key"/> does not begin with --</exception>
        /// <exception cref="InvalidOperationException">The key was specified without a value.</exception>
        /// <remarks>
        ///     The value should have a space between itself and the key, not an equals sign.
        /// </remarks>
        public T GetOptional<T>(string[] args, string key)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!key.StartsWith("--"))
            {
                throw new InvalidOperationException($"Key {key} does not start with --");
            }

            var i = GetIndexOfKey(args, key);
            if (i == -1)
            {
                return default(T);
            }
            else if (i == args.Length - 1)
            {
                throw new InvalidOperationException($"{key} was the last argument given, and does not have a value.");
            }

            var value = args[i + 1];
            if (value.StartsWith("--"))
            {
                throw new InvalidOperationException($"{key} was specified without a value.");
            }
            else
            {
                // Need to be able to cast to nullable.
                var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(value, type);
            }
        }

        /// <summary>
        ///     Returns a value from a list of arguments.
        /// </summary>
        /// <typeparam name="T">The type which the argument will be cast to.</typeparam>
        /// <param name="args">The list of arguments which may contain the key/value</param>
        /// <param name="key">The identifier of the argument.</param>
        /// <returns>
        ///     If the argument list has the key, the next value, otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="key"/> does not begin with --</exception>
        /// <exception cref="InvalidOperationException">The key was not found.</exception>
        /// <exception cref="InvalidOperationException">The key was specified without a value.</exception>
        /// <remarks>
        ///     The value should have a space between itself and the key, not an equals sign.
        /// </remarks>
        public T GetRequired<T>(string[] args, string key)
        {

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!key.StartsWith("--"))
            {
                throw new InvalidOperationException($"Key {key} does not start with --");
            }

            var i = GetIndexOfKey(args, key);
            if (i == -1)
            {
                throw new InvalidOperationException($"The required key {key} was not specified.");
            }
            else if (i == args.Length - 1)
            {
                throw new InvalidOperationException($"{key} was the last argument given, and does not have a value.");
            }

            var value = args[i + 1];
            if (value.StartsWith("--"))
            {
                throw new InvalidOperationException($"{key} was specified without a value.");
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary> 
        ///     Will return the index of the key, or -1 if not found. 
        /// </summary>
        private int GetIndexOfKey(string[] args, string key)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
