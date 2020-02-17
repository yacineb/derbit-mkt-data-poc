namespace deribit_mktdata.DeribitApiClient
{
    public static class ApiQueriesBuilder
    {
        public const string BtcInstrumentsQuery = 
            "{\"jsonrpc\":\"2.0\",\"id\":\"instr-1\",\"method\":\"public/get_instruments\",\"params\":{\"currency\":\"BTC\",\"expired\":false}}";

        public const string EthInstrumentsQuery = 
            "{\"jsonrpc\":\"2.0\",\"id\":\"instr-2\",\"method\":\"public/get_instruments\",\"params\":{\"currency\":\"ETH\",\"expired\":false}}";

        public static string GetIntrumentPriceQuery(string name)
        {
            return "{\"jsonrpc\":\"2.0\",\"id\":\"tr\",\"method\":\"public/get_last_trades_by_instrument\",\"params\":{\"instrument_name\":\""+name+"\",\"count\":1}}";
        }
    }
}