using System.Text.Json.Serialization;

namespace QuickChat.Message.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SystemMessageType
{
    UserJoined = 1
}
