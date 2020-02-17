using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deribit_mktdata.DeribitApiClient
{
    /**
     * A wrapper around ClientWebSocket native .net class
     */
    public class WsClient
    {
        private readonly ClientWebSocket _ws;
        private readonly WsClientOptions _opt;
        private readonly RateLimiter _rateLimiter;
        
        /**
         * A classic event handler is fine for this demo case. HOWEVER :
         * For Production Grade App i would use depending on the evolving technical/business requirements those components:
         *  - Reactive Extensions (http://reactivex.io/) for stream processing for its efficiency, robustness and great "Monadic" api
         *  - The new AsyncEnumerable is interesting too : https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
         *  - Or just the good old ConcurrentQueue if i just need to run consuming and producing of data into different threads
         *
         *  A Higher level of abstraction for concurrency has great benefits :
         *     - focusing on business logic/workflows
         *     - less technical boilerplate
         *     - easier testing, better error/exception handling
         */
        public event Func<string, Task> OnIncomingMessage;
        
        public WsClient(WsClientOptions opt)
        {
            _ws = new ClientWebSocket();
            _opt = opt;

            _rateLimiter = new RateLimiter(opt.RequestsLimitationTimeWindow, opt.RequestsLimitationNumber);
        }

        public async Task Connect(CancellationToken ct)
        {
            await _ws.ConnectAsync(new Uri(this._opt.Url), ct);
            ListenToIncomingResponses(ct);
        }

        /**
         * Sends a Request to the websocket Respecting the rate limitation in <cref name="WsClientOptions"/>
         */
        public async Task Request(string req, CancellationToken ct)
        {
            await _rateLimiter.Await(ct);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(req), WebSocketMessageType.Text, true, ct);
        }

        private void ListenToIncomingResponses(CancellationToken ct)
        {
            Task.Run(async () =>
            {
                do
                {
                    var buffer = new ArraySegment<byte>(new byte[2048]);
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult result;

                        do
                        {
                            result = await _ws.ReceiveAsync(buffer, ct);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        } while (!result.EndOfMessage);


                        if (result.MessageType == WebSocketMessageType.Close || ct.IsCancellationRequested)
                            break;

                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            var response = await reader.ReadToEndAsync();
                            await HandleIncomingMessage(response);
                        }
                    }

                } while (true);
            }, ct);
        }
        
        public void Close()
        {
            _ws.Dispose();
        }

        private async Task HandleIncomingMessage(string arg)
        {
            var handler = OnIncomingMessage;
            if (handler != null)
                await handler(arg);
        }
    }
}