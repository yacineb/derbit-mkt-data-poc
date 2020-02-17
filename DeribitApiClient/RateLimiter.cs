using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace deribit_mktdata.DeribitApiClient
{
    /**
     * Home-Made In-Memory generic rate limiter
     *
     * Note: instances of Rate limiter do not share any state.
     */
    public class RateLimiter
    {
        private readonly int _spinningIntervalMs;
        private readonly ConcurrentQueue<DateTime> _requestTimeStamps = new ConcurrentQueue<DateTime>();

        public RateLimiter(TimeSpan interval, int maxRequests, int spinningIntervalMs = 100)
        {
            _spinningIntervalMs = spinningIntervalMs;
            this.Interval = interval;
            this.MaxRequests = maxRequests;

        }
        public TimeSpan Interval { get; }
        public int MaxRequests { get; }

        /**
         * 
         */
        public async Task Await(CancellationToken ct)
        {  
            // trim queue in order to keep the same maximum "time window"
            while(_requestTimeStamps.Count > 0 && _requestTimeStamps.TryPeek(out var date) && (DateTime.Now - date) > Interval){
                _requestTimeStamps.TryDequeue(out _);
            }

            DateTime now = DateTime.Now;
            _requestTimeStamps.Enqueue(now);

            var tsc = new TaskCompletionSource<bool>();

            do
            {
                // passing if we're below the rate limitation
                if (_requestTimeStamps.Count < MaxRequests || (DateTime.Now - now) >= Interval)
                {
                    tsc.SetResult(true);
                    break;
                }
                Console.WriteLine("waiting ..");
                await Task.Delay(_spinningIntervalMs, ct);
            } while (true);

            await tsc.Task;
        }

    }

}