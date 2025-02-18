using System;

namespace SystemZgloszen;

[Serializable]
public class WrongTicketDataFormatException : Exception
{
    public WrongTicketDataFormatException() { }
    public WrongTicketDataFormatException(string message) : base(message) { }
    public WrongTicketDataFormatException(string message, Exception inner) : base(message, inner) { }
}