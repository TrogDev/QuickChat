namespace QuickChat.Attachment.Application.Exceptions;

public class FileUploadException : Exception
{
    public FileUploadException()
        : base() { }

    public FileUploadException(string message)
        : base(message) { }

    public FileUploadException(string message, Exception innerException)
        : base(message, innerException) { }
}
