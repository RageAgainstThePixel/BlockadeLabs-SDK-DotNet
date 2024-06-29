using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    internal class AuthInfo
    {
        public AuthInfo(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidCredentialException(nameof(apiKey));
            }

            ApiKey = apiKey;
        }

        /// <summary>
        /// The API key, required to access the service.
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; }
    }
}
