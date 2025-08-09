using System.Net;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using QuickChat.Message.API.Exceptions;
using QuickChat.Message.API.Extensions;
using QuickChat.Message.Application.Commands;
using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Application.Queries;
using QuickChat.Message.Domain.Exceptions;

namespace QuickChat.Message.API.Grpc;

public class MessageService(ISender mediator, ILogger<MessageService> logger) : Message.MessageBase
{
    private readonly ISender mediator = mediator;
    private readonly ILogger<MessageService> logger = logger;

    public override async Task<GetChatMessagesReply> GetChatMessages(
        GetChatMessagesRequest request,
        ServerCallContext context
    )
    {
        GetChatMessagesQuery query =
            new(
                ParseGuid(request.ChatId, "chat_id"),
                request.Limit,
                request.HasCursor ? request.Cursor : null
            );

        IList<Domain.Entities.Message> messages = await mediator.Send(query);

        GetChatMessagesReply reply = new();
        reply.Messages.AddRange(messages.Select(m => m.ToProto()));
        return reply;
    }

    public override async Task<Empty> AddMessage(
        AddMessageRequest request,
        ServerCallContext context
    )
    {
        AddMessageCommand command =
            new(
                ParseGuid(request.ChatId, "chat_id"),
                ParseGuid(request.UserId, "user_id"),
                request.Text,
                request.AttachmentIds.Select(id => ParseGuid(id, "attachment_ids"))
            );

        await mediator.Send(command);
        return new Empty();
    }

    public override async Task<Empty> EditMessage(
        EditMessageRequest request,
        ServerCallContext context
    )
    {
        EditMessageCommand command =
            new(
                request.Id,
                ParseGuid(request.ChatId, "chat_id"),
                request.Text,
                request.AttachmentIds.Select(id => ParseGuid(id, "attachment_ids")),
                ParseGuid(request.ActorId, "actor_id")
            );

        try
        {
            await mediator.Send(command);
        }
        catch (ActionForbiddenException e)
        {
            logger.LogInformation(
                e,
                "The actor with ID {userId} can't delete the message with ID {messageId}",
                request.ActorId,
                request.Id
            );
            ApiExceptionModel exception =
                new()
                {
                    Error = nameof(ActionForbiddenException),
                    Status = (int)HttpStatusCode.Forbidden,
                    Title = "Permission denied",
                    Description =
                        $"The actor with ID {request.ActorId} can't edit the message with ID {request.Id}"
                };
            throw exception.ToRpcException(StatusCode.PermissionDenied);
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Message with id {id} was not found", request.Id);
            ApiExceptionModel exception =
                new()
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Error = nameof(EntityNotFoundException),
                    Title = "Message was not found",
                    Description = $"Message with id {request.Id} was not found"
                };
            throw exception.ToRpcException(StatusCode.NotFound);
        }

        return new Empty();
    }

    public override async Task<Empty> DeleteMessage(
        DeleteMessageRequest request,
        ServerCallContext context
    )
    {
        DeleteMessageCommand command =
            new(
                request.Id,
                ParseGuid(request.ChatId, "chat_id"),
                ParseGuid(request.ActorId, "actor_id")
            );

        try
        {
            await mediator.Send(command);
        }
        catch (ActionForbiddenException e)
        {
            logger.LogInformation(
                e,
                "The actor with ID {userId} can't delete the message with ID {messageId}",
                request.ActorId,
                request.Id
            );
            ApiExceptionModel exception =
                new()
                {
                    Error = nameof(ActionForbiddenException),
                    Status = (int)HttpStatusCode.Forbidden,
                    Title = "Permission denied",
                    Description =
                        $"The actor with ID {request.ActorId} can't delete the message with ID {request.Id}"
                };
            throw exception.ToRpcException(StatusCode.PermissionDenied);
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Message with id {id} was not found", request.Id);
            ApiExceptionModel exception =
                new()
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Error = nameof(EntityNotFoundException),
                    Title = "Message was not found",
                    Description = $"Message with id {request.Id} was not found"
                };
            throw exception.ToRpcException(StatusCode.NotFound);
        }

        return new Empty();
    }

    private Guid ParseGuid(string guid, string fieldName)
    {
        try
        {
            return new Guid(guid);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid {Field} format", fieldName);
            ApiExceptionModel exception =
                new()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Error = nameof(FormatException),
                    Title = $"Invalid {fieldName} format",
                    Description = $"The field should be correct Guid"
                };
            throw exception.ToRpcException(StatusCode.InvalidArgument);
        }
    }
}
