using System;
using System.Runtime.Serialization;


namespace DO
{
    [Serializable]
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException() : base() { }
        public AlreadyExistException(string message) : base(message) { }
        public AlreadyExistException(string message, Exception inner) : base(message, inner) { }
        protected AlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    [Serializable]
    public class DoesntExistException : Exception
    {
        public DoesntExistException() : base() { }
        public DoesntExistException(string message) : base(message) { }
        public DoesntExistException(string message, Exception inner) : base(message, inner) { }
        protected DoesntExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    [Serializable]
    public class LoadingException : Exception
    {
        string filePath;
        public LoadingException() : base() { }
        public LoadingException(string message) : base(message) { }
        public LoadingException(string message, Exception inner) : base(message, inner) { }

        public LoadingException(string path, string messege, Exception inner) => filePath = path;
        protected LoadingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    [Serializable]
    public class CalculateDistanceProblemException : Exception
    {
        public CalculateDistanceProblemException() : base() { }
        public CalculateDistanceProblemException(string message) : base(message) { }
        public CalculateDistanceProblemException(string message, Exception inner) : base(message, inner) { }
        protected CalculateDistanceProblemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
