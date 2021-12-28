using System;
using System.Runtime.Serialization;


namespace BO
{
    [Serializable]
    public class InvalidInputException : Exception
    {
        public string myMessage = " ";

        public InvalidInputException() : base() { }
        public InvalidInputException(string message) : base(message) { myMessage += message; }
        public InvalidInputException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected InvalidInputException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "InvalidInputException: " + myMessage + InnerException.Message;
        }
    }

    [Serializable]
    public class AddingException : Exception
    {
        public string myMessage = "";

        public AddingException() : base() { }
        public AddingException(string message) : base(message) { myMessage += message; }
        public AddingException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected AddingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "AddingException: " + myMessage + InnerException.Message;
        }
    }

    [Serializable]
    public class UpdateException : Exception
    {
        public string myMessage = "";

        public UpdateException() : base() { }
        public UpdateException(string message) : base(message) { myMessage += message; }
        public UpdateException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected UpdateException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "UpdateException: " + myMessage + InnerException.Message;
        }
    }

    [Serializable]
    public class GetException : Exception
    {
        public string myMessage = "";

        public GetException() : base() { }
        public GetException(string message) : base(message) { myMessage += message; }
        public GetException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected GetException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "GetException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class GetListException : Exception
    {
        public string myMessage = "";

        public GetListException() : base() { }
        public GetListException(string message) : base(message) { myMessage += message; }
        public GetListException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected GetListException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "GetListException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class CheckInDroneToChargeException : Exception
    {
        public string myMessage = "";

        public CheckInDroneToChargeException() : base() { }
        public CheckInDroneToChargeException(string message) : base(message) { myMessage += message; }
        public CheckInDroneToChargeException(string message, Exception inner) : base(message, inner) { }
        protected CheckInDroneToChargeException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "CheckInDroneToChargeException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class CheckOutDroneFromChargeException : Exception
    {
        public string myMessage = "";

        public CheckOutDroneFromChargeException() : base() { }
        public CheckOutDroneFromChargeException(string message) : base(message) { myMessage += message; }
        public CheckOutDroneFromChargeException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected CheckOutDroneFromChargeException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "CheckOutDroneFromChargeException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class BatteryException : Exception
    {
        public string myMessage = "";

        public BatteryException() : base() { }
        public BatteryException(string message) : base(message) { myMessage += message; }
        public BatteryException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected BatteryException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "BatteryException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class SchedulParcelException : Exception
    {
        public string myMessage = "";

        public SchedulParcelException() : base() { }
        public SchedulParcelException(string message) : base(message) { myMessage += message; }
        public SchedulParcelException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected SchedulParcelException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "SchedulParcelException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class FindClosestStationException : Exception
    {
        public string myMessage = "";

        public FindClosestStationException() : base() { }
        public FindClosestStationException(string message) : base(message) { myMessage += message; }
        public FindClosestStationException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected FindClosestStationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "FindClosestStationException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class DronesException : Exception
    {
        public string myMessage = "";

        public DronesException() : base() { }
        public DronesException(string message) : base(message) { myMessage += message; }
        public DronesException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected DronesException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "DronesException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class PickUpParcelException : Exception
    {
        public string myMessage = "";

        public PickUpParcelException() : base() { }
        public PickUpParcelException(string message) : base(message) { myMessage += message; }
        public PickUpParcelException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected PickUpParcelException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "PickUpParcelException: " + Message + InnerException.Message;
        }
    }

    [Serializable]
    public class DeliveryException : Exception
    {
        public string myMessage = "";

        public DeliveryException() : base() { }
        public DeliveryException(string message) : base(message) { myMessage += message; }
        public DeliveryException(string message, Exception inner) : base(message, inner) { myMessage += message; }
        protected DeliveryException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return "DeliveryException: " + Message + InnerException.Message;
        }
    }
}