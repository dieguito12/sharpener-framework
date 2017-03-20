using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PHttp
{
    /// <summary>
    /// Representative class of a server in a http connection.</summary>
    /// <remarks>
    /// Implements the IDisposable interface.</remarks>
    public class HttpServer : IDisposable
    {
        /// <summary>
        /// TCP packets listener;
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// Indicates if the server has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Port where the server will be running.</summary>
        /// <remarks>
        /// 0 value by default.</remarks>
        private int _port = 0;

        /// <summary>
        /// Indicates the state of the server.</summary>
        /// <remarks>
        /// Stopped state by default.</remarks>
        private HttpServerState _state = HttpServerState.Stopped;

        /// <summary>
        /// Event Handler when the state is changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Event fired when client change.
        /// </summary>
        private AutoResetEvent _clientsChangedEvent = new AutoResetEvent(false);
        
        /// <summary>
        /// Dictionary of all http clients
        /// </summary>
        private Dictionary<HttpClient, bool> _clients = new Dictionary<HttpClient, bool>();

        /// <summary>
        /// Object for the synchronous lock
        /// </summary>
        private Object _syncLock = new Object();

        /// <summary>
        /// Gets the timeout manager of the server.</summary>
        /// <value>
        /// HttpTimeoutManager object.</value>
        internal HttpTimeoutManager TimeoutManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the state of the server.</summary>
        /// <value>
        /// HttpServerState value.</value>
        internal HttpServerState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value != _state)
                {
                    HttpServerState previous = _state;
                    _state = value;
                    OnStateChanged(new HttpEventArgs(previous, _state));
                }
            }
        }

        /// <summary>
        /// Gets the servers utilities.</summary>
        /// <value>
        /// HttpServerUtility object.</value>
        internal HttpServerUtility ServerUtility
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the servers current endpoint.</summary>
        /// <value>
        /// IPEndPoint object.</value>
        public IPEndPoint EndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the reading buffer size of each packet.</summary>
        /// <value>
        /// int value.</value>
        public int ReadBufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the writing buffer size of each response.</summary>
        /// <value>
        /// int value.</value>
        public int WriteBufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the server banner.</summary>
        /// <value>
        /// string value.</value>
        public string ServerBanner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets limit of time of reading.</summary>
        /// <value>
        /// TimeSpan object.</value>
        public TimeSpan ReadTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets limit of time of writing.</summary>
        /// <value>
        /// TimeSpan object.</value>
        public TimeSpan WriteTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets limit of time of shutting down.</summary>
        /// <value>
        /// TimeSpan object.</value>
        public TimeSpan ShutdownTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets port where the server is running.</summary>
        /// <value>
        /// int value.</value>
        public int Port
        {
            get
            {
                return _port;
            }
            private set
            {
                _port = value;
            }
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
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

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="port">Port where the server will run.</param>
        public HttpServer(int port) : base()
        {
            Port = port;
        }

        /// <summary>
        /// Method invoked when the server changes its state.
        /// </summary>
        /// <param name="args">Event arguments</param>
        protected virtual void OnStateChanged(EventArgs args)
        {
            if (StateChanged == null)
            {
                throw new NullReferenceException("StateChanged must be different than null");
            }
            StateChanged.Invoke(this, args);
        }

        /// <summary>
        /// Disposes the server.
        /// </summary>
        public void Dispose()
        {
            if (!this._disposed)
            {
                if (State == HttpServerState.Started)
                {
                    Stop();
                }
                if (_clientsChangedEvent != null)
                {
                    _clientsChangedEvent.Close();
                    _clientsChangedEvent = null;
                }
                if (TimeoutManager != null)
                {
                    TimeoutManager.Dispose();
                    TimeoutManager = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            if (_state == HttpServerState.Stopped)
            {
                State = HttpServerState.Starting;
                Console.WriteLine("Server Starting at {0} ...", EndPoint.Address);

                TimeoutManager = new HttpTimeoutManager(this);
                TcpListener listener = new TcpListener(EndPoint);
                try
                {
                    listener.Start();
                    _listener = listener;
                    ServerUtility = new HttpServerUtility();
                    Console.WriteLine("Server Running at {0} ...", EndPoint.Address);
                }
                catch (Exception exception)
                {
                    State = HttpServerState.Stopped;
                    Console.WriteLine("Failed starting the server: {0}", exception.Message);
                    throw new PHttpException("Failed starting the server.", exception);
                }
                State = HttpServerState.Started;
                BeginAcceptTcpClient();
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                VerifyState(HttpServerState.Started);
                State = HttpServerState.Stopping;
                _listener.Stop();
                StopClients();
            }
            catch (Exception e)
            {
                State = HttpServerState.Started;
                Console.WriteLine("Failed stopping the server: {0}", e.Message);
                throw new PHttpException("Failed to stop HTTP server.");
            }
            State = HttpServerState.Stopped;
        }

        /// <summary>
        /// Verifies the new state.
        /// </summary>
        /// <param name="state">New state of the server.</param>
        private void VerifyState(HttpServerState state)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if (_state != state)
            {
                throw new InvalidOperationException("Expected server to be in the state");
            }
        }

        /// <summary>
        /// Method that begins to accepts tcp connections.
        /// </summary>
        private void BeginAcceptTcpClient()
        {
            TcpListener localListener = _listener;
            if (localListener != null)
            {
                localListener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }
        }

        /// <summary>
        /// Async callback called when a tcp client is connected.
        /// </summary>
        /// <param name="asyncResult">Result object of the async acction.</param>
        private void AcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                TcpListener listener = _listener;
                if (listener == null)
                {
                    return;
                }
                TcpClient tcpClient = listener.EndAcceptTcpClient(asyncResult);
                if (_state == HttpServerState.Stopped)
                {
                    tcpClient.Close();
                }
                HttpClient client = new HttpClient(this);
                RegisterClient(client);
                client.BeginRequest();
                BeginAcceptTcpClient();
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                //
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }   
        }


        internal void RaiseRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            OnRequestReceived(new HttpRequestEventArgs(context));
        }

        /// <summary>
        /// Event thrown when a request is received
        /// </summary>
        /// <param name="args"></param>
        public void OnRequestReceived(HttpRequestEventArgs args)
        {
            //
        }

        internal bool RaiseUnhandledException(HttpContext context, Exception exception)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var e = new HttpExceptionEventArgs(context, exception);
            OnUnhandledException(e);
            return e.Handled;
        }

        /// <summary>
        /// Event thrown then a Http exception is unhandled
        /// </summary>
        /// <param name="args"></param>
        private void OnUnhandledException(HttpExceptionEventArgs args)
        {

        }

        /// <summary>
        /// Unregisters a new client
        /// </summary>
        /// <param name="client">Http client.</param>
        internal void UnregisterClient(HttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException();
            }

            lock(_syncLock)
            {
                Debug.Assert(_clients.ContainsKey(client));
                _clients.Remove(client);
                _clientsChangedEvent.Set();
            }
        }

        /// <summary>
        /// Stops clients of sending packets.
        /// </summary>
        private void StopClients()
        {
            var shutdownStarted = DateTime.Now;
            bool forceShutdown = false;
            // Clients that are waiting for new requests are closed.

            List<HttpClient> clients;
            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.RequestClose();
            }

            // First give all clients a chance to complete their running requests.
            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }

                var shutdownRunning = DateTime.Now - shutdownStarted;

                if (shutdownRunning >= ShutdownTimeout)
                {
                    forceShutdown = true;
                    break;
                }
                _clientsChangedEvent.WaitOne(ShutdownTimeout - shutdownRunning);
            }

            if (!forceShutdown)
                return;

            // If there are still clients running after the timeout, their
            // connections will be forcibly closed.
            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.ForceClose();
            }

            // Wait for the registered clients to be cleared.
            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }
                _clientsChangedEvent.WaitOne();
            }
        }

        /// <summary>
        /// Registers a new client
        /// </summary>
        /// <param name="client">Http client.</param>
        private void RegisterClient(HttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentException();
            }

            lock (_syncLock)
            {
                _clients.Add(client, true);
                _clientsChangedEvent.Set();
            }
        }

        /// <summary>
        /// Unregisters a new client
        /// </summary>
        /// <param name="client">Http client.</param>
    }
}
