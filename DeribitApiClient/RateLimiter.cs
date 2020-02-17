using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace deribit_mktdata.DeribitApiClient
{
    public class RateLimiter
    {
        private readonly ConcurrentQueue<DateTime> _request = new ConcurrentQueue<DateTime>();

        public RateLimiter(TimeSpan interval, int maxRequests)
        {
            this.Interval = interval;
            this.MaxRequests = maxRequests;

        }
        public TimeSpan Interval { get; }
        public int MaxRequests { get; }

        public async Task wait()
        {  
            //trim queue
            while(_request.Count>0 && _request.TryPeek(out var date) && (DateTime.Now - date) > Interval){
                _request.TryDequeue(out var _);
            }

            DateTime now = DateTime.Now;
            _request.Enqueue(now);

            var tsc = new TaskCompletionSource<bool>();

            do
            {
                if (_request.Count < MaxRequests || (DateTime.Now - now) >= Interval)
                {
                    tsc.SetResult(true);
                    break;
                }
                Console.WriteLine("waiting ..");
                await Task.Delay(100);
            } while (true);

            await tsc.Task;
        }

    }

}