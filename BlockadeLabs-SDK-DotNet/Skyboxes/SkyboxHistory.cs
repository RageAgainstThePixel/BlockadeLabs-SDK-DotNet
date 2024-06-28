using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxHistory : BaseResponse, IListResponse<SkyboxInfo>
    {
        [JsonInclude]
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; private set; }

        [JsonInclude]
        [JsonPropertyName("has_more")]
        public bool HasMore { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<SkyboxInfo> Items { get; private set; }

        [JsonIgnore]
        public IReadOnlyList<SkyboxInfo> Skyboxes => Items;
    }
}
