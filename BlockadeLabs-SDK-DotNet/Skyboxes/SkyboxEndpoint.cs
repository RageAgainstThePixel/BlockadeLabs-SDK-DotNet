using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BlockadeLabsSDK
{
    public sealed class SkyboxEndpoint : BlockadeLabsBaseEndPoint
    {
        private class SkyboxInfoRequest
        {
            [JsonInclude]
            [JsonPropertyName("request")]
            public SkyboxInfo SkyboxInfo { get; private set; }
        }

        private class SkyboxOperation
        {
            [JsonInclude]
            [JsonPropertyName("success")]
            [JsonConverter(typeof(StringOrObjectConverter<bool>))]
            public dynamic Success { get; private set; }

            [JsonInclude]
            [JsonPropertyName("error")]
            public string Error { get; private set; }
        }

        public SkyboxEndpoint(BlockadeLabsClient client) : base(client) { }

        protected override string Root => string.Empty;

        /// <summary>
        /// Returns the list of predefined styles that can influence the overall aesthetic of your skybox generation.
        /// </summary>
        /// <param name="model">The <see cref="SkyboxModel"/> to get styles for.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of <see cref="SkyboxStyle"/>s.</returns>
        public async Task<IReadOnlyList<SkyboxStyle>> GetSkyboxStylesAsync(SkyboxModel model, CancellationToken cancellationToken = default)
        {
            var @params = new Dictionary<string, string> { { "model_version", ((int)model).ToString() } };
            using var response = await client.Client.GetAsync(GetUrl("skybox/styles", @params), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<SkyboxStyle>>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Returns the list of predefined styles that can influence the overall aesthetic of your skybox generation, sorted by style family.
        /// This route can be used in order to build a menu of styles sorted by family.
        /// </summary>
        /// <param name="model">Optional, The <see cref="SkyboxModel"/> to get styles for.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of <see cref="SkyboxStyle"/>s.</returns>
        public async Task<IReadOnlyList<SkyboxStyle>> GetSkyboxStyleFamiliesAsync(SkyboxModel? model = null, CancellationToken cancellationToken = default)
        {
            var @params = new Dictionary<string, string>();

            if (model.HasValue)
            {
                @params.Add("model_version", ((int)model).ToString());
            }

            using var response = await client.Client.GetAsync(GetUrl("skybox/families", @params), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<SkyboxStyle>>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Generate a skybox image.
        /// </summary>
        /// <param name="skyboxRequest"><see cref="SkyboxRequest"/>.</param>
        /// <param name="exportOption">Optional, <see cref="SkyboxExportOption"/>.</param>
        /// <param name="progressCallback">Optional, <see cref="IProgress{SkyboxInfo}"/> progress callback.</param>
        /// <param name="pollingInterval">Optional, polling interval in milliseconds.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="SkyboxInfo"/>.</returns>
        public async Task<SkyboxInfo> GenerateSkyboxAsync(SkyboxRequest skyboxRequest, SkyboxExportOption exportOption, IProgress<SkyboxInfo> progressCallback = null, int? pollingInterval = null, CancellationToken cancellationToken = default)
            => await GenerateSkyboxAsync(skyboxRequest, new[] { exportOption }, progressCallback, pollingInterval, cancellationToken);

        /// <summary>
        /// Generate a skybox image.
        /// </summary>
        /// <param name="skyboxRequest"><see cref="SkyboxRequest"/>.</param>
        /// <param name="exportOptions">Optional, <see cref="SkyboxExportOption"/>s.</param>
        /// <param name="progressCallback">Optional, <see cref="IProgress{SkyboxInfo}"/> progress callback.</param>
        /// <param name="pollingInterval">Optional, polling interval in milliseconds.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="SkyboxInfo"/>.</returns>
        public async Task<SkyboxInfo> GenerateSkyboxAsync(SkyboxRequest skyboxRequest, SkyboxExportOption[] exportOptions = null, IProgress<SkyboxInfo> progressCallback = null, int? pollingInterval = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(skyboxRequest.Prompt))
            {
                throw new ArgumentException("Prompt is required.", nameof(skyboxRequest));
            }

            var formData = new MultipartFormDataContent { { new StringContent(skyboxRequest.Prompt), "prompt" } };

            if (skyboxRequest.EnhancePrompt.HasValue)
            {
                formData.Add(new StringContent(skyboxRequest.EnhancePrompt.ToString()), "enhance_prompt");
            }

            if (skyboxRequest.Seed.HasValue)
            {
                formData.Add(new StringContent(skyboxRequest.Seed.ToString()), "seed");
            }

            if (skyboxRequest.SkyboxStyleId.HasValue)
            {
                formData.Add(new StringContent(skyboxRequest.SkyboxStyleId.ToString()), "skybox_style_id");
            }

            if (skyboxRequest.RemixImagineId.HasValue)
            {
                formData.Add(new StringContent(skyboxRequest.RemixImagineId.ToString()), "remix_imagine_id");
            }

            if (!string.IsNullOrWhiteSpace(skyboxRequest.WebhookUrl))
            {
                formData.Add(new StringContent(skyboxRequest.WebhookUrl), "webhook_url");
            }

            if (skyboxRequest.HqDepth.HasValue)
            {
                formData.Add(new StringContent(skyboxRequest.HqDepth.ToString()), "return_depth_hq");
            }

            if (skyboxRequest.ControlImage != null)
            {
                if (!string.IsNullOrWhiteSpace(skyboxRequest.ControlModel))
                {
                    formData.Add(new StringContent(skyboxRequest.ControlModel), "control_model");
                }

                using var imageData = new MemoryStream();
                await skyboxRequest.ControlImage.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
                formData.Add(new StreamContent(imageData), "control_image", skyboxRequest.ControlImageFileName);
                skyboxRequest.Dispose();
            }

            using var response = await client.Client.PostAsync(GetUrl("skybox"), formData, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, formData, cancellationToken).ConfigureAwait(false);
            var skyboxInfo = JsonSerializer.Deserialize<SkyboxInfo>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
            progressCallback?.Report(skyboxInfo);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(pollingInterval ?? 3000, CancellationToken.None).ConfigureAwait(false);
                skyboxInfo = await GetSkyboxInfoAsync(skyboxInfo, cancellationToken).ConfigureAwait(false);
                progressCallback?.Report(skyboxInfo);
                if (skyboxInfo.Status is Status.Pending or Status.Processing or Status.Dispatched) { continue; }
                break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                var cancelResult = await CancelSkyboxGenerationAsync(skyboxInfo, CancellationToken.None).ConfigureAwait(false);

                if (!cancelResult)
                {
                    throw new Exception($"Failed to cancel generation for {skyboxInfo.Id}");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (skyboxInfo.Status == Status.Abort)
            {
                throw new OperationCanceledException($"Generation aborted for skybox {skyboxInfo.Id}\n{skyboxInfo.ErrorMessage}\n{skyboxInfo}");
            }

            if (skyboxInfo.Status != Status.Complete)
            {
                throw new Exception($"Failed to generate skybox! {skyboxInfo.Id} -> {skyboxInfo.Status}\nError: {skyboxInfo.ErrorMessage}\n{skyboxInfo}");
            }

            skyboxInfo.SetResponseData(response.Headers, client);
            var exportTasks = new List<Task>();

            if (exportOptions != null)
            {
                exportTasks.AddRange(exportOptions.Select(exportOption => ExportSkyboxAsync(skyboxInfo, exportOption, null, pollingInterval, null, cancellationToken)));
            }
            else
            {
                exportTasks.Add(ExportSkyboxAsync(skyboxInfo, DefaultExportOptions.Equirectangular_PNG, null, pollingInterval, null, cancellationToken));
                exportTasks.Add(ExportSkyboxAsync(skyboxInfo, DefaultExportOptions.DepthMap_PNG, null, pollingInterval, null, cancellationToken));
            }

            await Task.WhenAll(exportTasks).ConfigureAwait(false);
            skyboxInfo = await GetSkyboxInfoAsync(skyboxInfo.Id, cancellationToken).ConfigureAwait(false);
            skyboxInfo.SetResponseData(response.Headers, client);
            progressCallback?.Report(skyboxInfo);
            return skyboxInfo;
        }

        /// <summary>
        /// Returns the skybox metadata for the given skybox id.
        /// </summary>
        /// <param name="id">Skybox Id.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="SkyboxInfo"/>.</returns>
        public async Task<SkyboxInfo> GetSkyboxInfoAsync(int id, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"imagine/requests/{id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxInfo = JsonSerializer.Deserialize<SkyboxInfoRequest>(responseAsString, BlockadeLabsClient.JsonSerializationOptions)?.SkyboxInfo;
            skyboxInfo.SetResponseData(response.Headers, client);
            return skyboxInfo;
        }


        /// <summary>
        /// Deletes a skybox by id.
        /// </summary>
        /// <param name="id">The id of the skybox.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if skybox was successfully deleted.</returns>
        public async Task<bool> DeleteSkyboxAsync(int id, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"imagine/deleteImagine/{id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxOp = JsonSerializer.Deserialize<SkyboxOperation>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);

            const string successStatus = "Item deleted successfully";

            if (skyboxOp is not { Success: successStatus })
            {
                throw new Exception($"Failed to delete skybox {id}!\n{skyboxOp?.Error}");
            }

            return skyboxOp.Success.Equals(successStatus);
        }

        /// <summary>
        /// Gets the previously generated skyboxes.
        /// </summary>
        /// <param name="parameters">Optional, <see cref="SkyboxHistoryParameters"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="SkyboxHistory"/>.</returns>
        public async Task<SkyboxHistory> GetSkyboxHistoryAsync(SkyboxHistoryParameters parameters = null, CancellationToken cancellationToken = default)
        {
            var historyRequest = parameters ?? new SkyboxHistoryParameters();

            var @params = new Dictionary<string, string>();

            if (historyRequest.StatusFilter.HasValue)
            {
                @params.Add("status", historyRequest.StatusFilter.ToString()!.ToLower());
            }

            if (historyRequest.Limit.HasValue)
            {
                @params.Add("limit", historyRequest.Limit.ToString());
            }

            if (historyRequest.Offset.HasValue)
            {
                @params.Add("offset", historyRequest.Offset.ToString());
            }

            if (historyRequest.Order.HasValue)
            {
                @params.Add("order", historyRequest.Order.ToString()!.ToUpper());
            }

            if (historyRequest.ImagineId.HasValue)
            {
                @params.Add("imagine_id", historyRequest.ImagineId.ToString());
            }

            if (!string.IsNullOrWhiteSpace(historyRequest.QueryFilter))
            {
                @params.Add("query", WebUtility.UrlEncode(historyRequest.QueryFilter));
            }

            if (!string.IsNullOrWhiteSpace(historyRequest.GeneratorFilter))
            {
                @params.Add("generator", WebUtility.UrlEncode(historyRequest.GeneratorFilter));
            }

            if (historyRequest.FavoritesOnly.HasValue &&
                historyRequest.FavoritesOnly.Value)
            {
                @params.Add("my_likes", historyRequest.FavoritesOnly.Value.ToString().ToLower());
            }

            if (historyRequest.GeneratedBy.HasValue)
            {
                @params.Add("api_key_id", historyRequest.GeneratedBy.Value.ToString());
            }

            if (historyRequest.SkyboxStyleId is > 0)
            {
                @params.Add("skybox_style_id", historyRequest.SkyboxStyleId.ToString());
            }

            using var response = await client.Client.GetAsync(GetUrl("imagine/myRequests", @params), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxHistory = JsonSerializer.Deserialize<SkyboxHistory>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
            skyboxHistory.SetResponseData(response.Headers, client);
            return skyboxHistory;
        }

        /// <summary>
        /// Cancels a pending skybox generation request by id.
        /// </summary>
        /// <param name="id">The id of the skybox.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if generation was cancelled.</returns>
        public async Task<bool> CancelSkyboxGenerationAsync(int id, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"imagine/requests/{id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxOp = JsonSerializer.Deserialize<SkyboxOperation>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);

            if (skyboxOp is not { Success: true })
            {
                throw new Exception($"Failed to cancel generation for skybox {id}!\n{skyboxOp?.Error}");
            }

            return true;
        }

        /// <summary>
        /// Cancels ALL pending skybox generation requests.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if all generations are cancelled.</returns>
        public async Task<bool> CancelAllPendingSkyboxGenerationsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl("imagine/requests/pending"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxOp = JsonSerializer.Deserialize<SkyboxOperation>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);

            if (skyboxOp is not { Success: true })
            {
                if (skyboxOp != null &&
                    skyboxOp.Error.Contains("You don't have any pending"))
                {
                    return false;
                }

                throw new Exception($"Failed to cancel all pending skybox generations!\n{skyboxOp?.Error}");
            }

            return true;
        }

        /// <summary>
        /// Returns the list of all available export types.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of available export types.</returns>
        public async Task<IReadOnlyList<SkyboxExportOption>> GetAllSkyboxExportOptionsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl("skybox/export"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<SkyboxExportOption>>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Exports the <see cref="SkyboxInfo"/> using the provided <see cref="SkyboxExportOption"/>.
        /// </summary>
        /// <param name="skyboxInfo">Skybox to export.</param>
        /// <param name="exportOption">Export option to use.</param>
        /// <param name="progressCallback">Optional, <see cref="IProgress{SkyboxInfo}"/> progress callback.</param>
        /// <param name="pollingInterval">Optional, polling interval in seconds.</param>
        /// <param name="webhookUrl">Optional, specify a webhook url to specify the destination for progress updates.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Updated <see cref="SkyboxInfo"/> with exported assets loaded into memory.</returns>
        public async Task<SkyboxInfo> ExportSkyboxAsync(SkyboxInfo skyboxInfo, SkyboxExportOption exportOption, IProgress<SkyboxExportRequest> progressCallback = null, int? pollingInterval = null, string webhookUrl = null, CancellationToken cancellationToken = default)
        {
            var request = new Dictionary<string, string>
            {
                { "skybox_id", skyboxInfo.ObfuscatedId },
                { "type_id", exportOption.Id.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(webhookUrl))
            {
                request.Add("webhook_url", webhookUrl);
            }

            using var payload = JsonSerializer.Serialize(request, BlockadeLabsClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("skybox/export"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            var exportRequest = JsonSerializer.Deserialize<SkyboxExportRequest>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
            progressCallback?.Report(exportRequest);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(pollingInterval ?? 3000, CancellationToken.None).ConfigureAwait(false);
                exportRequest = await GetExportRequestStatusAsync(exportRequest, CancellationToken.None);
                progressCallback?.Report(exportRequest);
                if (exportRequest.Status is Status.Pending or Status.Processing or Status.Dispatched) { continue; }
                break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                var cancelResult = await CancelSkyboxExportAsync(exportRequest, CancellationToken.None);

                if (!cancelResult)
                {
                    throw new Exception($"Failed to cancel export for {exportRequest.Id}");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (exportRequest.Status == Status.Abort)
            {
                throw new OperationCanceledException($"Export aborted for skybox {skyboxInfo.Id}\n{exportRequest.ErrorMessage}\n{exportRequest}");
            }

            if (exportRequest.Status != Status.Complete)
            {
                throw new Exception($"Failed to export skybox! {exportRequest.Id} -> {exportRequest.Status}\nError: {exportRequest.ErrorMessage}\n{exportRequest}");
            }

            skyboxInfo = await GetSkyboxInfoAsync(skyboxInfo.Id, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            skyboxInfo.SetResponseData(response.Headers, client);
            return skyboxInfo;
        }

        /// <summary>
        /// Gets the status of a specified <see cref="SkyboxExportRequest"/>.
        /// </summary>
        /// <param name="exportRequest">The export option to get the current status for.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Updated <see cref="SkyboxExportRequest"/> with latest information.</returns>
        public async Task<SkyboxExportRequest> GetExportRequestStatusAsync(SkyboxExportRequest exportRequest, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"skybox/export/{exportRequest.Id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            exportRequest = JsonSerializer.Deserialize<SkyboxExportRequest>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);
            exportRequest.SetResponseData(response.Headers, client);
            return exportRequest;
        }

        /// <summary>
        /// Cancels the specified <see cref="SkyboxExportRequest"/>.
        /// </summary>
        /// <param name="exportRequest">The export option to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if generation was cancelled.</returns>
        public async Task<bool> CancelSkyboxExportAsync(SkyboxExportRequest exportRequest, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"skybox/export/{exportRequest.Id}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var skyboxOp = JsonSerializer.Deserialize<SkyboxOperation>(responseAsString, BlockadeLabsClient.JsonSerializationOptions);

            if (skyboxOp is not { Success: true })
            {
                throw new Exception($"Failed to cancel export for request {exportRequest.Id}!\n{skyboxOp?.Error}");
            }

            return true;
        }
    }
}
