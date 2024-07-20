using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    public sealed class BlockadeLabsClient : IDisposable
    {
        /// <summary>
        /// Creates a new entry point to the BlockadeLabs API, handling auth and allowing access to the various API endpoints
        /// </summary>
        /// <param name="authentication">
        /// The API authentication information to use for API calls,
        /// or <see langword="null"/> to attempt to use the <see cref="BlockadeLabsAuthentication.Default"/>,
        /// potentially loading from environment vars or from a config file.
        /// </param>
        /// <param name="clientSettings">
        /// The API client settings for specifying a proxy domain,
        /// or <see langword="null"/> to attempt to use the <see cref="BlockadeLabsClientSettings.Default"/>,
        /// potentially loading from environment vars or from a config file.
        /// </param>
        /// <param name="httpClient">A <see cref="HttpClient"/>.</param>
        /// <exception cref="AuthenticationException">Raised when authentication details are missing or invalid.</exception>
        /// <remarks>
        /// <see cref="BlockadeLabsClient"/> implements <see cref="IDisposable"/> to manage the lifecycle of the resources it uses, including <see cref="HttpClient"/>.
        /// When you initialize <see cref="BlockadeLabsClient"/>, it will create an internal <see cref="HttpClient"/> instance if one is not provided.
        /// This internal HttpClient is disposed of when BlockadeLabsClient is disposed of.
        /// If you provide an external HttpClient instance to BlockadeLabsClient, you are responsible for managing its disposal.
        /// </remarks>
        public BlockadeLabsClient(BlockadeLabsAuthentication authentication = null, BlockadeLabsClientSettings clientSettings = null, HttpClient httpClient = null)
        {
            BlockadeLabsAuthentication = authentication ?? BlockadeLabsAuthentication.Default;
            BlockadeLabsClientSettings = clientSettings ?? BlockadeLabsClientSettings.Default;

            if (BlockadeLabsAuthentication?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet#authentication for details.");
            }

            Client = SetupClient(httpClient);
            SkyboxEndpoint = new SkyboxEndpoint(this);
        }

        ~BlockadeLabsClient() => Dispose(false);

        #region IDisposable

        private bool isDisposed;
        private bool isCustomClient;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                if (!isCustomClient)
                {
                    Client?.Dispose();
                }

                isDisposed = true;
            }
        }

        #endregion IDisposable

        /// <summary>
        /// <see cref="HttpClient"/> to use when making calls to the API.
        /// </summary>
        internal HttpClient Client { get; }

        internal static JsonSerializerOptions JsonSerializationOptions { get; } = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverterFactory(),
            },
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public BlockadeLabsAuthentication BlockadeLabsAuthentication { get; }

        internal BlockadeLabsClientSettings BlockadeLabsClientSettings { get; }

        /// <summary>
        /// Enables or disables debugging for all endpoints.
        /// </summary>
        public bool EnableDebug { get; set; }

        public SkyboxEndpoint SkyboxEndpoint { get; }

        private HttpClient SetupClient(HttpClient client = null)
        {
            if (client == null)
            {
                client = new HttpClient(new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                });
            }
            else
            {
                isCustomClient = true;
            }

            client.DefaultRequestHeaders.Add("User-Agent", "BlockadeLabs-SDK-DotNet");
            client.DefaultRequestHeaders.Add("x-api-key", BlockadeLabsAuthentication.ApiKey);
            return client;
        }
    }
}
