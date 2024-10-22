﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlockadeLabsSDK
{
    internal static class ResponseExtensions
    {
        private const string RateLimit = "X-RateLimit-Limit";
        private const string RateLimitRemaining = "X-RateLimit-Remaining";

        private static readonly JsonSerializerOptions debugJsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers, BlockadeLabsClient client)
        {
            if (response is IListResponse<BaseResponse> listResponse)
            {
                foreach (var item in listResponse.Items)
                {
                    SetResponseData(item, headers, client);
                }
            }

            response.Client = client;

            if (headers == null) { return; }

            if (headers.TryGetValues(RateLimit, out var rateLimit) &&
                int.TryParse(rateLimit.First(), out var rateLimitValue))
            {
                response.RateLimit = rateLimitValue;
            }

            if (headers.TryGetValues(RateLimitRemaining, out var rateLimitRemaining) &&
                int.TryParse(rateLimitRemaining.First(), out var rateLimitRemainingValue))
            {
                response.RateLimitRemaining = rateLimitRemainingValue;
            }
        }

        internal static async Task CheckResponseAsync(this HttpResponseMessage response, bool debug, CancellationToken cancellationToken, [CallerMemberName] string methodName = null)
        {
            if (!response.IsSuccessStatusCode || debug)
            {
                await response.ReadAsStringAsync(debug, null, null, cancellationToken, methodName).ConfigureAwait(false);
            }
        }

        internal static async Task CheckResponseAsync(this HttpResponseMessage response, bool debug, HttpContent requestContent, CancellationToken cancellationToken, [CallerMemberName] string methodName = null)
        {
            if (!response.IsSuccessStatusCode || debug)
            {
                await response.ReadAsStringAsync(debug, requestContent, null, cancellationToken, methodName).ConfigureAwait(false);
            }
        }

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse, HttpContent requestContent, CancellationToken cancellationToken, [CallerMemberName] string methodName = null)
            => await response.ReadAsStringAsync(debugResponse, requestContent, null, cancellationToken, methodName).ConfigureAwait(false);

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse, CancellationToken cancellationToken, [CallerMemberName] string methodName = null)
            => await response.ReadAsStringAsync(debugResponse, null, null, cancellationToken, methodName).ConfigureAwait(false);

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, bool debugResponse, HttpContent requestContent, MemoryStream responseStream, CancellationToken cancellationToken, [CallerMemberName] string methodName = null)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var debugMessage = new StringBuilder();

            if (!response.IsSuccessStatusCode || debugResponse)
            {
                if (!string.IsNullOrWhiteSpace(methodName))
                {
                    debugMessage.Append($"{methodName} -> ");
                }

                var debugMessageObject = new Dictionary<string, Dictionary<string, object>>();

                if (response.RequestMessage != null)
                {
                    debugMessage.Append($"[{response.RequestMessage.Method}:{(int)response.StatusCode}] {response.RequestMessage.RequestUri}\n");

                    debugMessageObject["Request"] = new Dictionary<string, object>
                    {
                        ["Headers"] = response.RequestMessage.Headers.ToDictionary(pair => pair.Key, pair => pair.Value),
                    };
                }

                if (requestContent != null)
                {
                    debugMessageObject["Request"]["Body-Headers"] = requestContent.Headers.ToDictionary(pair => pair.Key, pair => pair.Value);
                    string requestAsString;

                    if (requestContent is MultipartFormDataContent multipartFormData)
                    {
                        var stringContents = multipartFormData.Select<HttpContent, object>(content =>
                        {
                            var headers = content.Headers.ToDictionary(pair => pair.Key, pair => pair.Value);
                            switch (content)
                            {
                                case StringContent stringContent:
                                    var valueAsString = stringContent.ReadAsStringAsync(cancellationToken).Result;
                                    object value;

                                    try
                                    {
                                        value = JsonNode.Parse(valueAsString);
                                    }
                                    catch
                                    {
                                        value = valueAsString;
                                    }

                                    return new { headers, value };
                                default:
                                    return new { headers };
                            }
                        });
                        requestAsString = JsonSerializer.Serialize(stringContents);
                    }
                    else
                    {
                        requestAsString = await requestContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    }

                    if (!string.IsNullOrWhiteSpace(requestAsString))
                    {
                        try
                        {
                            debugMessageObject["Request"]["Body"] = JsonNode.Parse(requestAsString);
                        }
                        catch
                        {
                            debugMessageObject["Request"]["Body"] = requestAsString;
                        }
                    }
                }

                debugMessageObject["Response"] = new()
                {
                    ["Headers"] = response.Headers.ToDictionary(pair => pair.Key, pair => pair.Value),
                };

                if (responseStream != null || !string.IsNullOrWhiteSpace(responseAsString))
                {
                    debugMessageObject["Response"]["Body"] = new Dictionary<string, object>();
                }

                if (responseStream != null)
                {
                    var body = Encoding.UTF8.GetString(responseStream.ToArray());

                    try
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Events"] = JsonNode.Parse(body);
                    }
                    catch
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Events"] = body;
                    }
                }

                if (!string.IsNullOrWhiteSpace(responseAsString))
                {
                    try
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Content"] = JsonNode.Parse(responseAsString);
                    }
                    catch
                    {
                        ((Dictionary<string, object>)debugMessageObject["Response"]["Body"])["Content"] = responseAsString;
                    }
                }

                debugMessage.Append(JsonSerializer.Serialize(debugMessageObject, debugJsonOptions));
                Console.WriteLine(debugMessage.ToString());
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(message: $"{methodName} Failed! HTTP status code: {response.StatusCode} | Response body: {responseAsString}", null, statusCode: response.StatusCode);
            }

            return responseAsString;
        }
    }
}
