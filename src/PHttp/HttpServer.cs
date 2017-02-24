using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PHttp
{
    public class HttpServer : IDisposable
    {
        public IPEndPoint EndPoint
        {
            get
            {
                return EndPoint;
            }
            set
            {
                EndPoint = value;
            }
        }
        public int ReadBufferSize
        {
            get
            {
                return ReadBufferSize;
            }
            set
            {
                ReadBufferSize = value;
            }
        }
        public int WriteBufferSize
        {
            get
            {
                return WriteBufferSize;
            }
            set
            {
                WriteBufferSize = value;
            }
        }
        public string ServerBanner
        {
            get
            {
                return ServerBanner;
            }
            set
            {
                ServerBanner = value;
            }
        }
        public TimeSpan ReadTimeout
        {
            get
            {
                return ReadTimeout;
            }
            set
            {
                ReadTimeout = value;
            }
        }
        public TimeSpan WriteTimeout
        {
            get
            {
                return WriteTimeout;
            }
            set
            {
                WriteTimeout = value;
            }
        }
        public TimeSpan ShutdownTimeout
        {
            get
            {
                return ShutdownTimeout;
            }
            set
            {
                ShutdownTimeout = value;
            }
        }

        internal HttpServerUtility ServerUtility;

        internal HttpTimeoutManager TimeoutManager;

        public HttpServer()
        {
            EndPoint = new IPEndPoint(IPAddress.Loopback, 0);
            ReadBufferSize = 4096;
            WriteBufferSize = 4096;
            ShutdownTimeout = TimeSpan.FromSeconds(30);
            ReadTimeout = TimeSpan.FromSeconds(90);
            WriteTimeout = TimeSpan.FromSeconds(90);
            ServerBanner = String.Format("PHttp/{0}", GetType().Assembly.GetName().Version);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }
        private void VerifyState(HttpServerState state)
        {
            throw new NotImplementedException();
        }
        private void StopClients()
        {
            throw new NotImplementedException();
        }
        private void BeginAcceptTcpClient()
        {
            throw new NotImplementedException();
        }
        private void AcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
        private void RegisterClient(HttpClient client)
        {
            throw new NotImplementedException();
        }
        internal void UnregisterClient(HttpClient client)
        {
            throw new NotImplementedException();
        }
    }
}
