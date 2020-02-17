using System;
using System.Threading;
using System.Threading.Tasks;
using deribit_mktdata.DeribitApiClient;
using deribit_mktdata.Jobs;
using deribit_mktdata.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace deribit_mktdata
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var interval = TimeSpan.FromSeconds(80);
            await RunJob(interval, CancellationToken.None);
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:5000");
                });


        private static async Task RunJob(TimeSpan interval, CancellationToken ct)
        {
            var client = new WsClient(new WsClientOptions());
            await client.Connect(ct);

            var process = new DeribitQuotesRetriever(client, new InstrumentsDataRepository(new DatabaseSettings()));
            process.Run(interval,ct);
        }
    }
}