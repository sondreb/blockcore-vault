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
        public void Get_AllTheMoney_Success()
        {
            var money = this.fixture.Services.GetService<IMoney>();

            Assert.Equal(int.MaxValue, money.GetAll());
        }
    }
}
