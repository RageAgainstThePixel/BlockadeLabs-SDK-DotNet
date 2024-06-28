using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace BlockadeLabsSDK.Tests
{
    internal abstract class AbstractTestFixture
    {
        protected class TestProxyFactory : WebApplicationFactory<Proxy.Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Development");
                base.ConfigureWebHost(builder);
            }
        }

        internal const string TestUserToken = "aAbBcCdDeE123456789";

        protected readonly HttpClient HttpClient;

        protected readonly BlockadeLabsClient BlockadeLabsClient;

        protected AbstractTestFixture()
        {
            var webApplicationFactory = new TestProxyFactory();
            HttpClient = webApplicationFactory.CreateClient();
            var domain = $"{HttpClient.BaseAddress?.Authority}:{HttpClient.BaseAddress?.Port}";
            var settings = new BlockadeLabsSettings(domain);
            var auth = new BlockadeLabsAuthentication(TestUserToken);
            HttpClient.Timeout = TimeSpan.FromMinutes(3);

            BlockadeLabsClient = new BlockadeLabsClient(auth, settings)
            {
                EnableDebug = true
            };
        }
    }
}
