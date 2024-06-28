using System.IO;
using System;
using System.Text.Json;

namespace BlockadeLabsSDK
{
    public sealed class BlockadeLabsAuthentication
    {
        internal const string CONFIG_FILE = ".blockadelabs";
        private const string BLOCKADELABS_API_KEY = nameof(BLOCKADELABS_API_KEY);
        private const string BLOCKADE_LABS_API_KEY = nameof(BLOCKADE_LABS_API_KEY);

        private readonly AuthInfo authInfo;

        /// <summary>
        /// The API key, required to access the API endpoint.
        /// </summary>
        public string ApiKey => authInfo.ApiKey;

        /// <summary>
        /// Allows implicit casting from a string, so that a simple string API key can be provided in place of an instance of <see cref="BlockadeLabsAuthentication"/>.
        /// </summary>
        /// <param name="key">The API key to convert into a <see cref="BlockadeLabsAuthentication"/>.</param>
        public static implicit operator BlockadeLabsAuthentication(string key) => new BlockadeLabsAuthentication(key);

        private BlockadeLabsAuthentication(AuthInfo authInfo) => this.authInfo = authInfo;

        /// <summary>
        /// Instantiates a new Authentication object with the given <paramref name="apiKey"/>, which may be <see langword="null"/>.
        /// </summary>
        /// <param name="apiKey">The API key, required to access the API endpoint.</param>
        public BlockadeLabsAuthentication(string apiKey) => authInfo = new AuthInfo(apiKey);

        private static BlockadeLabsAuthentication cachedDefault;

        /// <summary>
        /// The default authentication to use when no other auth is specified.
        /// This can be set manually, or automatically loaded via environment variables or a config file.
        /// <seealso cref="LoadFromEnvironment"/><seealso cref="LoadFromDirectory"/>
        /// </summary>
        public static BlockadeLabsAuthentication Default
        {
            get
            {
                if (cachedDefault != null)
                {
                    return cachedDefault;
                }

                var auth = LoadFromDirectory() ??
                           LoadFromDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) ??
                           LoadFromEnvironment();
                cachedDefault = auth;
                return auth;
            }
            internal set => cachedDefault = value;
        }

        /// <summary>
        /// Attempts to load api keys from environment variables, as "ELEVEN_LABS_API_KEY"
        /// </summary>
        /// <returns>
        /// Returns the loaded <see cref="BlockadeLabsAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if there were no matching environment vars.
        /// </returns>
        public static BlockadeLabsAuthentication LoadFromEnvironment()
        {
            var apiKey = Environment.GetEnvironmentVariable(BLOCKADELABS_API_KEY);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(BLOCKADE_LABS_API_KEY);
            }

            return string.IsNullOrEmpty(apiKey) ? null : new BlockadeLabsAuthentication(apiKey);
        }

        /// <summary>
        /// Attempts to load api keys from a specified configuration file.
        /// </summary>
        /// <param name="path">The specified path to the configuration file.</param>
        /// <returns>
        /// Returns the loaded <see cref="BlockadeLabsAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static BlockadeLabsAuthentication LoadFromPath(string path)
            => LoadFromDirectory(Path.GetDirectoryName(path), Path.GetFileName(path), false);

        /// <summary>
        /// Attempts to load api keys from a configuration file, by default ".elevenlabs" in the current directory,
        /// optionally traversing up the directory tree.
        /// </summary>
        /// <param name="directory">
        /// The directory to look in, or <see langword="null"/> for the current directory.
        /// </param>
        /// <param name="filename">
        /// The filename of the config file.
        /// </param>
        /// <param name="searchUp">
        /// Whether to recursively traverse up the directory tree if the <paramref name="filename"/> is not found in the <paramref name="directory"/>.
        /// </param>
        /// <returns>
        /// Returns the loaded <see cref="BlockadeLabsAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static BlockadeLabsAuthentication LoadFromDirectory(string directory = null, string filename = CONFIG_FILE, bool searchUp = true)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = CONFIG_FILE;
            }

            AuthInfo authInfo = null;

            var currentDirectory = new DirectoryInfo(directory);

            while (authInfo == null && currentDirectory.Parent != null)
            {
                var filePath = Path.Combine(currentDirectory.FullName, filename);

                if (File.Exists(filePath))
                {
                    try
                    {
                        authInfo = JsonSerializer.Deserialize<AuthInfo>(File.ReadAllText(filePath));
                        break;
                    }
                    catch (Exception)
                    {
                        // try to parse the old way for backwards support.
                    }

                    var lines = File.ReadAllLines(filePath);
                    string apiKey = null;

                    foreach (var line in lines)
                    {
                        var parts = line.Split('=', ':');

                        for (var i = 0; i < parts.Length - 1; i++)
                        {
                            var part = parts[i];
                            var nextPart = parts[i + 1];

                            switch (part)
                            {
                                case BLOCKADELABS_API_KEY:
                                case BLOCKADE_LABS_API_KEY:
                                    apiKey = nextPart.Trim();
                                    break;
                            }
                        }
                    }

                    authInfo = new AuthInfo(apiKey);
                }

                if (searchUp)
                {
                    currentDirectory = currentDirectory.Parent;
                }
                else
                {
                    break;
                }
            }

            return string.IsNullOrEmpty(authInfo?.ApiKey) ? null : new BlockadeLabsAuthentication(authInfo);
        }
    }
}
