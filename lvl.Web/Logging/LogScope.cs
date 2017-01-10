using System;
using System.Threading;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Allows for log messages to be nested.
    /// </summary>
    internal class LogScope
    {
        private string Name { get; }
        private object State { get; }

        public LogScope(string name, object state)
        {
            Name = name;
            State = state;
        }

        public LogScope Parent { get; private set; }

        private static AsyncLocal<LogScope> _current { get; set; } = new AsyncLocal<LogScope>();
        public static LogScope Current
        {
            get
            {
                return _current.Value;
            }
            set
            {
                _current.Value = value;
            }
        }

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new LogScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        public override string ToString() => State?.ToString();

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
