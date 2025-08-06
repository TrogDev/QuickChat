namespace QuickChat.Chat.Domain.Exceptions;

public class UserAlreadyJoinedException : Exception
{
    public UserAlreadyJoinedException()
        : base() { }

    public UserAlreadyJoinedException(string message)
        : base(message) { }

    public UserAlreadyJoinedException(string message, Exception innerException)
        : base(message, innerException) { }
}
