using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
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
            throw new RpcException(
                new Status(
                    StatusCode.PermissionDenied,
                    $"The actor with ID {request.ActorId} can't delete the message with ID {request.Id}"
                )
            );
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Message with id {id} was not found", request.Id);
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Message with id {request.Id} was not found")
            );
        }

        return new Empty();
    }

    public override async Task<Empty> DeleteMessage(
        DeleteMessageRequest request,
        ServerCallContext context
    )
    {
        DeleteMessageCommand command = new(request.Id, ParseGuid(request.ActorId, "actor_id"));

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
            throw new RpcException(
                new Status(
                    StatusCode.PermissionDenied,
                    $"The actor with ID {request.ActorId} can't delete the message with ID {request.Id}"
                )
            );
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Message with id {id} was not found", request.Id);
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Message with id {request.Id} was not found")
            );
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
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, $"Invalid {fieldName} format")
            );
        }
    }
}
