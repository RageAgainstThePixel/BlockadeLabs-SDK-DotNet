using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxInfo : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public int Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("obfuscated_id")]
        public string ObfuscatedId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("skybox_style_id")]
        public int SkyboxStyleId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("skybox_style_name")]
        public string SkyboxStyleName { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        [JsonConverter(typeof(JsonStringEnumConverter<SkyboxModel>))]
        public SkyboxModel Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter<Status>))]
        public Status Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("queue_position")]
        public int QueuePosition { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_url")]
        public string MainTextureUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("thumb_url")]
        public string ThumbnailUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("depth_map_url")]
        public string DepthTextureUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("title")]
        public string Title { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prompt")]
        public string Prompt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("negative_text")]
        public string NegativeText { get; private set; }

        [JsonInclude]
        [JsonPropertyName("seed")]
        public int Seed { get; private set; }

        [JsonInclude]
        [JsonPropertyName("remix_imagine_id")]
        public int? RemixId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("remix_obfuscated_id")]
        public string RemixObfuscatedId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("isMyFavorite")]
        public bool IsMyFavorite { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("dispatched_at")]
        public DateTime DispatchedAt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("processing_at")]
        public DateTime ProcessingAt { get; private set; }

        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public DateTime CompletedAt { get; private set; }

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
        [JsonPropertyName("exports")]
        public IReadOnlyDictionary<string, string> Exports { get; private set; }

        public static implicit operator int(SkyboxInfo info) => info.Id;

        public override string ToString() => JsonSerializer.Serialize(this, BlockadeLabsClient.JsonSerializationOptions);
    }
}
