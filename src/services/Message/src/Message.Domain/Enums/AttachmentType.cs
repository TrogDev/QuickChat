using System.Text.Json.Serialization;

namespace QuickChat.Message.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttachmentType
{
    Image = 1,
    Video = 2,
    File = 3
}
