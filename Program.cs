using System;
using System.Threading;
using deribit_mktdata.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace deribit_mktdata
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            JobsFactory.StartInstrumentsQuotesJob(TimeSpan.FromSeconds(80), CancellationToken.None);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:5000");
                });

    }
}