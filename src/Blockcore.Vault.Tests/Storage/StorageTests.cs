using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Blockcore.Vault.Storage;
using Microsoft.AspNetCore.TestHost;
using Blockcore.Vault.Tests.Fakes;

namespace Blockcore.Vault.Tests.Storage
{
    public class StorageTests : IClassFixture<AppTestFixture>
    {
        readonly AppTestFixture fixture;
        // readonly HttpClient client;
        private static HttpClient client;

        public StorageTests(AppTestFixture fixture)
        {
            this.fixture = fixture;

            // Reuse the client cross tests.
            client ??= fixture.CreateClient();
        }

        [Fact]
        public void Get_AllTheMoneyUnit_Success()
        {
            var money = this.fixture.Services.GetService<IMoney>();

            Assert.Equal(int.MaxValue, money.GetAll());
        }

        [Fact]
        public async void Get_AllTheMoney_Override_Success()
        {
            var response = await client.GetAsync("/api/data/all");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(int.MaxValue.ToString(), await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async void Get_AllTheMoney_Success()
        {
            // Arrange with override of IMoney
            var client = fixture.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMoney, TestMoney>();
                });
            }).CreateClient();

            var response = await client.GetAsync("/api/data/all");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("0", await response.Content.ReadAsStringAsync());
        }
    }
}
