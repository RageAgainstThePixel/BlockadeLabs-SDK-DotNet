using NUnit.Framework;
using System;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;

namespace BlockadeLabsSDK.Tests
{
    internal class TestFixture_00_Authentication
    {
        [SetUp]
        public void Setup()
        {
            var authJson = new AuthInfo("key-test12");
            var authText = JsonSerializer.Serialize(authJson);
            File.WriteAllText(BlockadeLabsAuthentication.CONFIG_FILE, authText);
            Assert.IsTrue(File.Exists(BlockadeLabsAuthentication.CONFIG_FILE));
        }

        [Test]
        public void Test_01_GetAuthFromEnv()
        {
            var auth = BlockadeLabsAuthentication.LoadFromEnvironment();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
        }

        [Test]
        public void Test_02_GetAuthFromFile()
        {
            var auth = BlockadeLabsAuthentication.LoadFromPath(
                Path.GetFullPath(BlockadeLabsAuthentication.CONFIG_FILE));

            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-test12", auth.ApiKey);
        }

        [Test]
        public void Test_03_GetAuthFromNonExistentFile()
        {
            var auth = BlockadeLabsAuthentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void Test_04_GetDefault()
        {
            var auth = BlockadeLabsAuthentication.Default;
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-test12", auth.ApiKey);
        }

        [Test]
        public void Test_05_Authentication()
        {
            var defaultAuth = BlockadeLabsAuthentication.Default;
            var manualAuth = new BlockadeLabsAuthentication("key-testAA");
            var api = new BlockadeLabsClient();
            var shouldBeDefaultAuth = api.BlockadeLabsAuthentication;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);

            BlockadeLabsAuthentication.Default = new BlockadeLabsAuthentication("key-testAA");
            using var api2 = new BlockadeLabsClient();
            var shouldBeManualAuth = api2.BlockadeLabsAuthentication;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);

            BlockadeLabsAuthentication.Default = defaultAuth;
        }

        [Test]
        public void Test_06_GetKey()
        {
            var auth = new BlockadeLabsAuthentication("key-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-testAA", auth.ApiKey);
        }

        [Test]
        public void Test_07_GetKeyFailed()
        {
            BlockadeLabsAuthentication auth = null;

            try
            {
                auth = new BlockadeLabsAuthentication("fail-key");
            }
            catch (InvalidCredentialException)
            {
                Assert.IsNull(auth);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false,
                    $"Expected exception {nameof(InvalidCredentialException)} but got {e.GetType().Name}");
            }
        }

        [Test]
        public void Test_08_ParseKey()
        {
            var auth = new BlockadeLabsAuthentication("key-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-testAA", auth.ApiKey);
            auth = "key-testCC";
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-testCC", auth.ApiKey);

            auth = new BlockadeLabsAuthentication("key-testBB");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("key-testBB", auth.ApiKey);
        }

        [Test]
        public void Test_12_CustomDomainConfigurationSettings()
        {
            var auth = new BlockadeLabsAuthentication("customIssuedToken");
            var settings = new BlockadeLabsClientSettings(domain: "api.your-custom-domain.com");
            var api = new BlockadeLabsClient(auth, settings);
            Console.WriteLine(api.BlockadeLabsClientSettings.BaseRequest);
            Console.WriteLine(api.BlockadeLabsClientSettings.BaseRequestUrlFormat);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(BlockadeLabsAuthentication.CONFIG_FILE))
            {
                File.Delete(BlockadeLabsAuthentication.CONFIG_FILE);
            }


            BlockadeLabsClientSettings.Default = null;
            BlockadeLabsAuthentication.Default = null;
        }
    }
}
