using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlockadeLabsSDK.Tests
{
    internal class TestFixture_01_Skyboxes : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_GetSkyboxStyles()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);

            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model2);
            Assert.IsNotNull(skyboxStyles);

            foreach (var skyboxStyle in skyboxStyles)
            {
                if (skyboxStyle.FamilyStyles != null)
                {
                    Console.WriteLine($"family: {skyboxStyle}");
                    foreach (var familyStyle in skyboxStyle.FamilyStyles)
                    {
                        Console.WriteLine($"Style: {familyStyle}");
                        Assert.IsTrue(familyStyle.Model == SkyboxModel.Model2);
                    }
                }
                else
                {
                    Console.WriteLine($"Style: {skyboxStyle}");
                    Assert.IsTrue(skyboxStyle.Model == SkyboxModel.Model2);
                }
            }

            var skyboxFamilyStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStyleFamiliesAsync(SkyboxModel.Model3);
            Assert.IsNotNull(skyboxFamilyStyles);

            foreach (var skyboxStyle in skyboxFamilyStyles)
            {
                if (skyboxStyle.FamilyStyles != null)
                {
                    Console.WriteLine($"family: {skyboxStyle}");
                    foreach (var familyStyle in skyboxStyle.FamilyStyles)
                    {
                        Console.WriteLine($"Style: {familyStyle}");
                        Assert.IsTrue(familyStyle.Model == SkyboxModel.Model3);
                    }
                }
                else
                {
                    Console.WriteLine($"Style: {skyboxStyle}");
                    Assert.IsTrue(skyboxStyle.Model == SkyboxModel.Model3);
                }
            }
        }

        [Test]
        [Timeout(600000)] // 10 min timeout
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

            skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxInfo.ObfuscatedId);
            Assert.IsNotNull(skyboxInfo);
            Assert.IsNotEmpty(skyboxInfo.Exports);

            foreach (var exportInfo in skyboxInfo.Exports)
            {
                Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
            }

            var exportOptions = await BlockadeLabsClient.SkyboxEndpoint.GetAllSkyboxExportOptionsAsync();
            Assert.IsNotNull(exportOptions);
            Assert.IsNotEmpty(exportOptions);
            var exportTasks = new List<Task>();

            foreach (var exportOption in exportOptions)
            {
                exportTasks.Add(ExportAsync(skyboxInfo));

                async Task ExportAsync(SkyboxInfo exportInfo)
                {
                    Console.WriteLine(exportOption.Key);
                    Assert.IsNotNull(exportOption);
                    var skyboxExport = await BlockadeLabsClient.SkyboxEndpoint.ExportSkyboxAsync(exportInfo, exportOption);
                    Assert.IsNotNull(skyboxExport);
                    Assert.IsTrue(skyboxExport.Exports.ContainsKey(exportOption.Key));
                    skyboxExport.Exports.TryGetValue(exportOption.Key, out var exportUrl);
                    Console.WriteLine(exportUrl);
                }
            }

            await Task.WhenAll(exportTasks);
            skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxInfo);
            Assert.IsTrue(skyboxInfo.Exports.Count == exportTasks.Count);

            if (skyboxInfo.Exports.Count > 0)
            {
                foreach (var exportInfo in skyboxInfo.Exports)
                {
                    Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
                }
            }
        }

        [Test]
        [Timeout(600000)] // 10 min timeout
        public async Task Test_03_GenerateSkyboxRemix()
        {
            Assert.IsNotNull(BlockadeLabsClient.SkyboxEndpoint);
            var blockadeLogoPath = Path.GetFullPath("../../../Assets/BlockadeLabs-SDK-DotNet-Icon.png");
            Console.WriteLine(blockadeLogoPath);
            Assert.IsTrue(File.Exists(blockadeLogoPath));
            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
            var request = new SkyboxRequest(skyboxStyles.First(), "mars", controlImagePath: blockadeLogoPath, enhancePrompt: true);
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
            var exportTasks = new List<Task>();

            foreach (var exportOption in exportOptions)
            {
                exportTasks.Add(ExportAsync(skyboxInfo));

                async Task ExportAsync(SkyboxInfo exportInfo)
                {
                    Console.WriteLine(exportOption.Key);
                    Assert.IsNotNull(exportOption);
                    var skyboxExport = await BlockadeLabsClient.SkyboxEndpoint.ExportSkyboxAsync(exportInfo, exportOption);
                    Assert.IsNotNull(skyboxExport);
                    Assert.IsTrue(skyboxExport.Exports.ContainsKey(exportOption.Key));
                    skyboxExport.Exports.TryGetValue(exportOption.Key, out var exportUrl);
                    Console.WriteLine(exportUrl);
                }
            }

            await Task.WhenAll(exportTasks).ConfigureAwait(true);
            skyboxInfo = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxInfo);
            Assert.IsTrue(skyboxInfo.Exports.Count == exportTasks.Count);

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
            var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
            var request = new SkyboxRequest(skyboxStyles.First(), "mars", enhancePrompt: true);

            var progress = new Progress<SkyboxInfo>(async progress =>
            {
                Console.WriteLine(progress?.Status);
                var result = await BlockadeLabsClient.SkyboxEndpoint.CancelAllPendingSkyboxGenerationsAsync();
                Console.WriteLine(result ? "All pending generations successfully cancelled" : "No pending generations");
            });

            try
            {
                await BlockadeLabsClient.SkyboxEndpoint.GenerateSkyboxAsync(request, progressCallback: progress, pollingInterval: 0.5f);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation successfully cancelled");
            }
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
