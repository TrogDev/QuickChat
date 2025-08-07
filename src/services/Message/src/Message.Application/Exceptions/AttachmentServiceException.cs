namespace QuickChat.Message.Application.Exceptions;

public class AttachmentServiceException : Exception
{
    public AttachmentServiceException()
        : base() { }

    public AttachmentServiceException(string message)
        : base(message) { }

    public AttachmentServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}
