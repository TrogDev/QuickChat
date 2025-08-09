using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using QuickChat.Chat.API.Extensions;
using QuickChat.Chat.Application.Commands;
using QuickChat.Chat.Application.Exceptions;
using QuickChat.Chat.Application.Queries;
using QuickChat.Chat.Domain.Entities;
using QuickChat.Chat.Domain.Exceptions;

namespace QuickChat.Chat.API.Grpc;

public class ChatService(ISender mediator, ILogger<ChatService> logger) : Chat.ChatBase
{
    private readonly ISender mediator = mediator;
    private readonly ILogger<ChatService> logger = logger;

    public override async Task<GetUserChatsReply> GetUserChats(
        GetUserChatsRequest request,
        ServerCallContext context
    )
    {
        GetUserChatsQuery query = new(ParseGuid(request.UserId, "user_id"));
        IList<Domain.Entities.Chat> chats = await mediator.Send(query);
        GetUserChatsReply reply = new();
        reply.Chats.AddRange(chats.Select(c => c.ToProto()));
        return reply;
    }

    public override async Task<GetChatByCodeReply> GetChatByCode(
        GetChatByCodeRequest request,
        ServerCallContext context
    )
    {
        GetChatByCodeQuery query = new(request.Code);

        Domain.Entities.Chat chat;

        try
        {
            chat = await mediator.Send(query);
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Chat with code {code} was not found", request.Code);
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Chat with code {request.Code} was not found")
            );
        }

        GetChatByCodeReply reply = new() { Chat = chat.ToProto() };
        return reply;
    }

    public override async Task<JoinChatReply> JoinChat(
        JoinChatRequest request,
        ServerCallContext context
    )
    {
        JoinChatCommand command =
            new(
                ParseGuid(request.ChatId, "chat_id"),
                ParseGuid(request.UserId, "user_id"),
                request.Name
            );

        ChatParticipant participant;

        try
        {
            participant = await mediator.Send(command);
        }
        catch (UserAlreadyJoinedException e)
        {
            logger.LogInformation(e, "The user has already joined");
            throw new RpcException(
                new Status(StatusCode.AlreadyExists, "The user has already joined")
            );
        }
        catch (EntityNotFoundException e)
        {
            logger.LogInformation(e, "Chat with id {id} was not found", request.ChatId);
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Chat with id {request.ChatId} was not found")
            );
        }

        JoinChatReply reply = new() { Participant = participant.ToProto() };

        return reply;
    }

    public override async Task<CreateChatReply> CreateChat(
        CreateChatRequest request,
        ServerCallContext context
    )
    {
        CreateChatCommand command = new(request.Name);
        Domain.Entities.Chat chat = await mediator.Send(command);
        CreateChatReply reply = new() { Chat = chat.ToProto() };
        return reply;
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
