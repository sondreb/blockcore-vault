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

namespace Blockcore.Vault.Tests.Controllers
{
    public class SyncControllerTests : IClassFixture<AppTestFixture>
    {
        readonly AppTestFixture fixture;
        private static HttpClient client;

        public SyncControllerTests(AppTestFixture fixture)
        {
            this.fixture = fixture;

            // Reuse the client cross tests.
            client ??= fixture.CreateClient();
        }

        [Theory]
        [InlineData("/api/sync/items?PageNumber=1&PageSize=10")]
        public async void Get_SyncAllItems_Success(string url)
        {
            // Act
            var response = await client.GetAsync(url);

            // Assert1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert2
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}
