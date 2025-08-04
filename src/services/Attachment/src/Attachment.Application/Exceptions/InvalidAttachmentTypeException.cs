namespace QuickChat.Attachment.Application.Exceptions;

public class InvalidAttachmentTypeException : Exception
{
    public InvalidAttachmentTypeException()
        : base() { }

    public InvalidAttachmentTypeException(string message)
        : base(message) { }

    public InvalidAttachmentTypeException(string message, Exception innerException)
        : base(message, innerException) { }
}
