using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxExportOption : BaseResponse
    {
        public const string Equirectangular_JPG = "equirectangular-jpg";
        public const string Equirectangular_PNG = "equirectangular-png";
        public const string CubeMap_Roblox_PNG = "cube-map-roblox-png";
        public const string HDRI_HDR = "hdri-hdr";
        public const string HDRI_EXR = "hdri-exr";
        public const string DepthMap_PNG = "depth-map-png";
        public const string Video_LandScape_MP4 = "video-landscape-mp4";
        public const string Video_Portrait_MP4 = "video-portrait-mp4";
        public const string Video_Square_MP4 = "video-square-mp4";
        public const string CubeMap_PNG = "cube-map-default-png";

        public SkyboxExportOption() { }

        public SkyboxExportOption(int id, string name, string key)
        {
            Id = id;
            Name = name;
            Key = key;
        }

        [JsonConstructor]
        internal SkyboxExportOption(int id, string name, string key, bool isPremium)
        {
            Id = id;
            Name = name;
            Key = key;
            IsPremium = isPremium;
        }

        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonInclude]
        [JsonPropertyName("id")]
        public int Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("key")]
        public string Key { get; private set; }

        [JsonInclude]
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; private set; }
    }
}
