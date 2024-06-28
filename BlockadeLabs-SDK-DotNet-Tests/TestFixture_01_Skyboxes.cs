﻿using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace BlockadeLabsSDK.Tests
{
    internal class TestFixture_01_Skyboxes : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_GetSkyboxStyles()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);

            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
            Assert.IsNotNull(skyboxStyles);

            foreach (var skyboxStyle in skyboxStyles)
            {
                Console.WriteLine(skyboxStyle);
            }

            var skyboxFamilyStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStyleFamiliesAsync(SkyboxModel.Model3);
            Assert.IsNotNull(skyboxFamilyStyles);

            foreach (var skyboxStyle in skyboxFamilyStyles)
            {
                Console.WriteLine(skyboxStyle);
            }
        }

        [Test] // 10 min timeout
        [Timeout(600000)]
        public async Task Test_02_GenerateSkybox()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);

            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
            var request = new SkyboxRequest(skyboxStyles.First(), "mars", enhancePrompt: true);
            var skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.GenerateSkyboxAsync(request);
            Assert.IsNotNull(skyboxInfo);
            Console.WriteLine($"Successfully created skybox: {skyboxInfo.Id}");

            Assert.IsNotEmpty(skyboxInfo.Exports);

            foreach (var exportInfo in skyboxInfo.Exports)
            {
                Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
            }

            Console.WriteLine(skyboxInfo.ToString());

            skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxInfo);
            Assert.IsNotNull(skyboxInfo);
            Assert.IsNotEmpty(skyboxInfo.Exports);

            foreach (var exportInfo in skyboxInfo.Exports)
            {
                Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
            }

            var exportOptions = await BlockadeLabsClient.SkyboxEndpoint.GetAllSkyboxExportOptionsAsync();
            Assert.IsNotNull(exportOptions);
            Assert.IsNotEmpty(exportOptions);

            foreach (var exportOption in exportOptions)
            {
                Console.WriteLine(exportOption.Key);
                Assert.IsNotNull(exportOption);
                skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.ExportSkyboxAsync(skyboxInfo, exportOption);
                Assert.IsNotNull(skyboxInfo);
                Assert.IsTrue(skyboxInfo.Exports.ContainsKey(exportOption.Key));
                skyboxInfo.Exports.TryGetValue(exportOption.Key, out var exportUrl);
                Console.WriteLine(exportUrl);
            }

            if (skyboxInfo.Exports.Count > 0)
            {
                foreach (var exportInfo in skyboxInfo.Exports)
                {
                    Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
                }
            }
        }

        [Test]
        public async Task Test_04_GetHistory()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var history = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxHistoryAsync();
            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history.Skyboxes);
            Console.WriteLine($"Found {history.TotalCount} skyboxes");

            foreach (var skybox in history.Skyboxes)
            {
                Console.WriteLine($"[{skybox.ObfuscatedId}] {skybox.Title} status: {skybox.Status} @ {skybox.CompletedAt}");
            }
        }

        [Test]
        public async Task Test_05_CancelPendingGeneration()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
            var request = new SkyboxRequest(skyboxStyles.First(), "mars", enhancePrompt: true);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1.5));

            try
            {
                await BlockadeLabsClient.SkyboxEndpoint.GenerateSkyboxAsync(request, pollingInterval: 1, cancellationToken: cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation successfully cancelled");
            }
        }

        [Test]
        public async Task Test_06_CancelAllPendingGenerations()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var result = await BlockadeLabsClient.SkyboxEndpoint.CancelAllPendingSkyboxGenerationsAsync();
            Console.WriteLine(result ? "All pending generations successfully cancelled" : "No pending generations");
        }

        [Test]
        public async Task Test_07_DeleteSkybox()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var history = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxHistoryAsync(new SkyboxHistoryParameters { StatusFilter = Status.Abort });
            Assert.IsNotNull(history);

            foreach (var skybox in history.Skyboxes)
            {
                Console.WriteLine($"Deleting {skybox.Id} {skybox.Title}");
                var result = await BlockadeLabsClient.SkyboxEndpoint.DeleteSkyboxAsync(skybox);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public async Task Test_08_GetSkyboxExportOptions()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var exportOptions = await BlockadeLabsClient.SkyboxEndpoint.GetAllSkyboxExportOptionsAsync();
            Assert.IsNotNull(exportOptions);
            Assert.IsNotEmpty(exportOptions);

            foreach (var exportOption in exportOptions)
            {
                Console.WriteLine(JsonConvert.SerializeObject(exportOption));
            }
        }
    }
}
