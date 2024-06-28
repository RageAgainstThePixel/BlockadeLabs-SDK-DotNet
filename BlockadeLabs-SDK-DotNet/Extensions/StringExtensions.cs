// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net.Http;

namespace BlockadeLabsSDK
{
    internal static class StringExtensions
    {
        public static StringContent ToJsonStringContent(this string json)
        {
            const string jsonContent = "application/json";
            return new StringContent(json, null, jsonContent);
        }
    }
}
