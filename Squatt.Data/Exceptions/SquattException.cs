using System;

namespace Piec.Info.Squatt.Data.Exceptions
{
    public class SquattException : Exception
    {
        public SquattException()
            : base()
        {
        }

        public SquattException(string message)
            : base(message)
        {
        }

        public SquattException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}