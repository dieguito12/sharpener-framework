using System;
using System.Runtime.Serialization;

namespace PHttp
{
    /// <summary>
    /// Custom exception class of the web server
    /// </summary>
    [Serializable]
    public class PHttpException : Exception
    {
        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public PHttpException()
        {
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="message">Message to show.</param>
        public PHttpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <param name="innerException">Inner exception</param>
        public PHttpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}