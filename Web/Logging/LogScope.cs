using System;
using System.Threading;

namespace lvl.Web.Logging
{
    /// <summary>
    ///     Allows for log messages to be nested.
    /// </summary>
    internal class LogScope
    {
        private object State { get; }

        public LogScope(object state)
        {
            State = state;
        }

        public LogScope Parent { get; private set; }

        // ReSharper disable once InconsistentNaming Resharper getting confused.
        private static AsyncLocal<LogScope> _current { get; } = new AsyncLocal<LogScope>();
        public static LogScope Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }

        public static IDisposable Push(object state)
        {
            var temp = Current;
            Current = new LogScope(state) { Parent = temp };

            return new DisposableScope();
        }

        public override string ToString() => State.ToString();

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
