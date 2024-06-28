using System.Text.Json.Serialization;
using System;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxExportRequest : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_url")]
        public string FileUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("skybox_obfuscated_id")]
        public string SkyboxObfuscatedId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type_id")]
        public int TypeId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<Status>))]
        public Status Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("queue_position")]
        public int QueuePosition { get; private set; }

        [JsonInclude]
        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; private set; }

        [JsonInclude]
        [JsonPropertyName("pusher_channel")]
        public string PusherChannel { get; private set; }

        [JsonInclude]
        [JsonPropertyName("pusher_event")]
        public string PusherEvent { get; private set; }

        [JsonInclude]
        [JsonPropertyName("webhook_url")]
        public string WebhookUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; private set; }
    }
}
