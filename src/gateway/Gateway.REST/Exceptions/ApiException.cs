using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Exceptions;

/// <summary>
/// Message will be displayed to the user; don't put sensitive data here
/// </summary>
public class ApiException : Exception
{
    public ApiExceptionModel Model { get; }

    public ApiException(ApiExceptionModel model)
        : base(model.Title)
    {
        Model = model;
    }

    public ApiException(ApiExceptionModel model, Exception innerException)
        : base(model.Title, innerException)
    {
        Model = model;
    }
}
