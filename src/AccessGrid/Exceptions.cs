using System;

namespace AccessGrid
{
    /// <summary>
    /// Base exception for AccessGrid SDK
    /// </summary>
    public class AccessGridException : Exception
    {
        public AccessGridException(string message) : base(message) { }
        public AccessGridException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Raised when authentication fails
    /// </summary>
    public class AuthenticationException : AccessGridException
    {
        public AuthenticationException(string message) : base(message) { }
    }
}