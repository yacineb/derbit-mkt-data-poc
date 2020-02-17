using System;

namespace deribit_mktdata.DeribitApiClient
{
    public class WsClientOptions
    {
        /**
         * Ws Api Endpoint
         */
        public string Url = "wss://test.deribit.com/ws/api/v2";

        /**
         * Limit of requests number in the given RequestsLimitationTimeWindow
         */
        
        public int RequestsLimitationNumber  = 20;

        /**
         * The time window in which ratelimiter operates
         */
        public TimeSpan RequestsLimitationTimeWindow = TimeSpan.FromSeconds(10);
    }
}