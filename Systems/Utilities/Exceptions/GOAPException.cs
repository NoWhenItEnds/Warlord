using Godot;
using System;

namespace Warlord.Utilities.Exceptions
{
    /// <summary> An exception thrown by a GOAP component. </summary>
    public class GOAPException : Exception
    {
        /// <summary> An exception thrown by a GOAP action strategy. </summary>
        public GOAPException() { }


        /// <summary> An exception thrown by a GOAP component. </summary>
        /// <param name="message"> The exception's error message. </param>
        public GOAPException(String message) : base(message) { }


        /// <summary> An exception thrown by a GOAP component. </summary>
        /// <param name="message"> The exception's error message. </param>
        /// <param name="innerException"> The exception wrapped by this one. </param>
        public GOAPException(String message, Exception innerException) : base(message, innerException) { }
    }
}
