using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.Vault.Tests
{
    public class AppTestFixture : WebApplicationFactory<Startup>
    {
        public AppTestFixture()
        {

        }

        //override methods here as needed
        //protected override IHostBuilder CreateHostBuilder()
        //{
        //    return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
        //    builder.UseStartup<Startup>());
        //}
    }
}
