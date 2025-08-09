namespace QuickChat.Gateway.REST.Exceptions;

public class MissingUserClaimException : Exception
{
    public MissingUserClaimException()
        : base() { }

    public MissingUserClaimException(string message)
        : base(message) { }

    public MissingUserClaimException(string message, Exception innerException)
        : base(message, innerException) { }
}
