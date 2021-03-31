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
    public class DataControllerTests : IClassFixture<AppTestFixture>
    {
        readonly AppTestFixture fixture;
        // readonly HttpClient client;
        private static HttpClient client;

        public DataControllerTests(AppTestFixture fixture)
        {
            this.fixture = fixture;

            // Reuse the client cross tests.
            client ??= fixture.CreateClient();
        }

        [Theory]
        [InlineData("/api/data/1")]
        [InlineData("/api/data/5000")]
        public async void Get_DataListAll_Success(string url)
        {
            // Act
            var response = await client.GetAsync(url);

            // Assert1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert2
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/data")]
        public async void Get_DataListAll_Failure(string url)
        {
            // Act
            var response = await client.GetAsync(url);

            // Assert: Should be bad request.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
