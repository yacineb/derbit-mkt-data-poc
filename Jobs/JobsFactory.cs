using System;
using System.Threading;
using System.Threading.Tasks;
using deribit_mktdata.DeribitApiClient;
using deribit_mktdata.Storage;
using WebApi.Storage;

namespace deribit_mktdata.Jobs
{
    public static class JobsFactory
    {
        /**
         * Runs the job which retrieves instruments prices at the given <paramref name="interval"/>
         */
        public static async Task StartInstrumentsQuotesJob(TimeSpan interval, CancellationToken ct)
        {
            var options = new WsClientOptions();
            
            using var client = new WsClient(options);
            await client.Connect(ct);

            var process = new DeribitQuotesRetriever(client, new InstrumentsDataRepository(new DatabaseSettings()));
            await process.Run(interval,ct);
        }
    }
}