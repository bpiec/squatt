using System;

namespace Dabarto.Data.Squatt.Data.Exceptions
{
    public class SquattConfigurationException : Exception
    {
        public SquattConfigurationException()
            : base()
        {
        }

        public SquattConfigurationException(string message)
            : base(message)
        {
        }

        public SquattConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}