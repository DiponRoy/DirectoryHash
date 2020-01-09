using System;
using System.Runtime.Serialization;

namespace CompareDirectoryHash
{
    [Serializable]
    internal class ILGenerateException : Exception
    {
        public ILGenerateException()
        {
        }

        public ILGenerateException(string message) : base(message)
        {
        }

        public ILGenerateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ILGenerateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}