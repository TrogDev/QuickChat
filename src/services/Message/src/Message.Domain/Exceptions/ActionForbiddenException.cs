namespace QuickChat.Message.Domain.Exceptions;

public class ActionForbiddenException : Exception
{
    public ActionForbiddenException()
        : base() { }

    public ActionForbiddenException(string message)
        : base(message) { }

    public ActionForbiddenException(string message, Exception innerException)
        : base(message, innerException) { }
}
