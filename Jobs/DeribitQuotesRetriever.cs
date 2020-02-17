using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using deribit_mktdata.DeribitApiClient;
using deribit_mktdata.Storage;
using Newtonsoft.Json.Linq;
using WebApi.Storage;

namespace deribit_mktdata.Jobs
{
    public class DeribitQuotesRetriever
    {
        private readonly WsClient _client;
        private readonly InstrumentsDataRepository _repository;

        public DeribitQuotesRetriever(WsClient client, InstrumentsDataRepository repository)
        {
            _client = client;
            _repository = repository;
            _client.OnIncomingMessage += ProcessResponse;
        }

        private async Task QueryInstrumentsPrices(IEnumerable<string> instruments)
        {
            if (instruments == null)
                return;
            
            foreach (var name in instruments)
            {
                await _client.Request(ApiQueriesBuilder.GetIntrumentPriceQuery(name), CancellationToken.None);
            }
        }
        
        private async Task ProcessResponse(string json)
        {
            JObject o = JObject.Parse(json);

            // if the the response is of type instruments list then retrieve their ids and query their prices
            var reqId = o.Value<string>("id");
            if (reqId.StartsWith("instr"))
            {
                var result = o.Value<JArray>("result");
                var instruments = result.Values<string>("instrument_name");
                await QueryInstrumentsPrices(instruments);
            }
            else if (reqId.StartsWith("tr"))
            {
                var instrumentQuote = o.Value<JObject>("result").Value<JArray>("trades").Select(t=> new InstrumentData()
                {
                    Name = t.Value<string>("instrument_name"),
                    Price= t.Value<decimal>("price"),
                    Timestamp = t.Value<long>("timestamp"),
                }).FirstOrDefault();
                
                if (instrumentQuote != null)
                {
                    await _repository.Add(instrumentQuote);
                }
            }
        }
        
        private async Task FetchInstrumentsList(CancellationToken ct)
        {
            await _client.Request(ApiQueriesBuilder.BtcInstrumentsQuery, ct);
            await _client.Request(ApiQueriesBuilder.EthInstrumentsQuery, ct);
        }

        /**
         * Runs the data retrieval with a given execution <paramref name="interval"/>.
         *  This background job queries Derebit Api and persists data to the storage
         */
        public Task Run(TimeSpan interval, CancellationToken ct)
        {
            return Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        await FetchInstrumentsList(ct);
                        await Task.Delay(interval, ct);
                    }
                    catch (TaskCanceledException e)
                    {
                        return;
                    }
                } while (true);
            }, ct);
        }
        

    }
}