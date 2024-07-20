using System.Collections.Generic;

namespace BlockadeLabsSDK
{
    public sealed class BlockadeLabsClientSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsClientSettings"/> with default <see cref="BlockadeLabsClientSettingsInfo"/>.
        /// </summary>
        public BlockadeLabsClientSettings()
        {
            Info = new BlockadeLabsClientSettingsInfo();
            cachedDefault = this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsClientSettings"/> with the provided <see cref="clientSettingsInfo"/>.
        /// </summary>
        /// <param name="clientSettingsInfo"><see cref="BlockadeLabsClientSettingsInfo"/>.</param>
        public BlockadeLabsClientSettings(BlockadeLabsClientSettingsInfo clientSettingsInfo)
        {
            Info = clientSettingsInfo;
            cachedDefault = this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsClientSettings"/>.
        /// </summary>
        /// <param name="domain">Base api domain.</param>
        public BlockadeLabsClientSettings(string domain)
        {
            Info = new BlockadeLabsClientSettingsInfo(domain);
            cachedDefault = this;
        }

        private static BlockadeLabsClientSettings cachedDefault;

        public static BlockadeLabsClientSettings Default
        {
            get => cachedDefault ??= new BlockadeLabsClientSettings();
            internal set => cachedDefault = value;
        }

        public BlockadeLabsClientSettingsInfo Info { get; }

        internal string BaseRequest => Info.BaseRequest;

        internal string BaseRequestUrlFormat => Info.BaseRequestUrlFormat;

        private readonly Dictionary<string, string> defaultQueryParameters = new();

        internal IReadOnlyDictionary<string, string> DefaultQueryParameters => defaultQueryParameters;
    }
}
