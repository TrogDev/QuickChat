using Grpc.Core;
using MediatR;
using QuickChat.Message.API.Extensions;
using QuickChat.Message.Application.Queries;

namespace QuickChat.Message.API.Grpc;

public class SystemMessageService(ISender mediator, ILogger<SystemMessageService> logger)
    : SystemMessage.SystemMessageBase
{
    private readonly ISender mediator = mediator;
    private readonly ILogger<SystemMessageService> logger = logger;

    public override async Task<GetChatSystemMessagesReply> GetChatSystemMessages(
        GetChatSystemMessagesRequest request,
        ServerCallContext context
    )
    {
        GetChatSystemMessagesQuery query =
            new(
                ParseGuid(request.ChatId, "chat_id"),
                request.Limit,
                request.HasCursor ? request.Cursor : null
            );

        IList<Domain.Entities.SystemMessage> messages = await mediator.Send(query);

        GetChatSystemMessagesReply reply = new();
        reply.Messages.AddRange(messages.Select(m => m.ToProto()));
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
