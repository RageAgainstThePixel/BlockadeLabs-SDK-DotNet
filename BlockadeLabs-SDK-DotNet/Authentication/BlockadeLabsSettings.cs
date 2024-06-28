using System.Collections.Generic;

namespace BlockadeLabsSDK
{
    public sealed class BlockadeLabsSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsSettings"/> with default <see cref="BlockadeLabsSettingsInfo"/>.
        /// </summary>
        public BlockadeLabsSettings()
        {
            Info = new BlockadeLabsSettingsInfo();
            cachedDefault = this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsSettings"/> with the provided <see cref="settingsInfo"/>.
        /// </summary>
        /// <param name="settingsInfo"><see cref="BlockadeLabsSettingsInfo"/>.</param>
        public BlockadeLabsSettings(BlockadeLabsSettingsInfo settingsInfo)
        {
            Info = settingsInfo;
            cachedDefault = this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BlockadeLabsSettings"/>.
        /// </summary>
        /// <param name="domain">Base api domain.</param>
        public BlockadeLabsSettings(string domain)
        {
            Info = new BlockadeLabsSettingsInfo(domain);
            cachedDefault = this;
        }

        private static BlockadeLabsSettings cachedDefault;

        public static BlockadeLabsSettings Default
        {
            get => cachedDefault ??= new BlockadeLabsSettings();
            internal set => cachedDefault = value;
        }

        public BlockadeLabsSettingsInfo Info { get; }

        public string BaseRequestUrlFormat => Info.BaseRequestUrlFormat;

        private readonly Dictionary<string, string> defaultQueryParameters = new();

        internal IReadOnlyDictionary<string, string> DefaultQueryParameters => defaultQueryParameters;
    }
}
