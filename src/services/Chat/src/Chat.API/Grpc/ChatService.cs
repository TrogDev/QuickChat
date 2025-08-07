using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using QuickChat.Chat.API.Extensions;
using QuickChat.Chat.Application.Commands;
using QuickChat.Chat.Application.Exceptions;
using QuickChat.Chat.Application.Queries;
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
        GetUserChatsQuery query;

        try
        {
            query = new(new Guid(request.UserId));
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid UserId format");
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid UserId format"));
        }

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

    public override async Task<Empty> JoinChat(JoinChatRequest request, ServerCallContext context)
    {
        JoinChatCommand command;

        try
        {
            command = new(request.Code, new Guid(request.UserId), request.Name);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid UserId format");
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid UserId format"));
        }

        try
        {
            await mediator.Send(command);
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
            logger.LogInformation(e, "Chat with code {code} was not found", request.Code);
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Chat with code {request.Code} was not found")
            );
        }

        return new Empty();
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
}
