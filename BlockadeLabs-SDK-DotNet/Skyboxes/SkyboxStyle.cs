using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxStyle : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonInclude]
        [JsonPropertyName("id")]
        public int Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description { get; private set; }

        [JsonInclude]
        [JsonPropertyName("max-char")]
        public int? MaxChar { get; private set; }

        [JsonInclude]
        [JsonPropertyName("negative-text-max-char")]
        public int? NegativeTextMaxChar { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image")]
        public string Image { get; private set; }

        [JsonInclude]
        [JsonPropertyName("sort_order")]
        public int SortOrder { get; private set; }

        [JsonInclude]
        [JsonPropertyName("premium")]
        public bool Premium { get; private set; }

        [JsonInclude]
        [JsonPropertyName("new")]
        public bool New { get; private set; }

        [JsonInclude]
        [JsonPropertyName("experimental")]
        public bool Experimental { get; private set; }

        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model")]
        [JsonConverter(typeof(JsonStringEnumConverter<SkyboxModel>))]
        public SkyboxModel? Model { get; private set; }

        [JsonInclude]
        [JsonPropertyName("model_version")]
        public int? ModelVersion { get; private set; }

        [JsonInclude]
        [JsonPropertyName("items")]
        public IReadOnlyList<SkyboxStyle> FamilyStyles { get; private set; }

        public static implicit operator int(SkyboxStyle style) => style.Id;

        public override string ToString() => JsonSerializer.Serialize(this, BlockadeLabsClient.JsonSerializationOptions);
    }
}
