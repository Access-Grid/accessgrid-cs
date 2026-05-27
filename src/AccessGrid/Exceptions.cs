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

    /// <summary>
    /// Thrown when a SmartTap reveal envelope is missing required fields,
    /// contains non-base64 / non-PEM data, or otherwise can't be parsed
    /// before the cryptographic operations begin.
    /// </summary>
    public class InvalidEnvelopeException : AccessGridException
    {
        public InvalidEnvelopeException(string message) : base(message) { }
        public InvalidEnvelopeException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when AES-GCM auth-tag verification fails while decrypting a
    /// SmartTap reveal envelope (wrong key, tampered envelope, or wire-format
    /// drift between server and SDK).
    /// </summary>
    public class DecryptException : AccessGridException
    {
        public DecryptException(string message) : base(message) { }
        public DecryptException(string message, Exception innerException) : base(message, innerException) { }
    }
}