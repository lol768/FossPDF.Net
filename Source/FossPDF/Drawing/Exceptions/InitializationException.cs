using System;

namespace FossPDF.Drawing.Exceptions
{
    public class InitializationException : Exception
    {
        internal InitializationException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
