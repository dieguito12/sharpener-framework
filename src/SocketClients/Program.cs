using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketClients
{

    class SynchronousClient
    {
        private static IPAddress host;
        private const int port = 1337;

        public SynchronousClient(string stringHost)
        {
            host = IPAddress.Parse(stringHost);
        }

        public string SendMessage(string message)
        {
            int bytes = 0;
            byte[] buffer = new byte[512];
            StringBuilder response = new StringBuilder();
            Socket socket = CreateSocket(host);
            string stringHost = host.ToString();
            SendPackage(socket, message);
            do
            {
                bytes = socket.Receive(buffer);
                response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
            } while (bytes > 0);
            socket.Close();
            return response.ToString();
        }

        private Socket CreateSocket(IPAddress hostEntry)
        {
            IPEndPoint endPoint = new IPEndPoint(hostEntry, port);
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            if (socket.Connected)
            {
                return socket;
            }
            return null;
        }

        private void SendPackage(Socket socket, string message)
        {
            byte[] requestBytes = Encoding.ASCII.GetBytes(message);
            socket.Send(requestBytes);
        }
    }

    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    class AsynchronousClient
    {
        private static IPAddress host;
        private const int port = 1337;
        private static StringBuilder response = new StringBuilder();

        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public AsynchronousClient(string stringHost)
        {
            host = IPAddress.Parse(stringHost);
        }

        public string SendMessage(string message)
        {
            int bytes = 0;
            byte[] buffer = new byte[512];
            StringBuilder response = new StringBuilder();
            Socket socket = CreateSocket(host);
            string stringHost = host.ToString();
            SendPackage(socket, message);
            StateObject state = new StateObject();
            state.workSocket = socket;

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            receiveDone.WaitOne();

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return response.ToString();
        }


        private static void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject) ar.AsyncState;
            Socket client = state.workSocket;
            int bytes = 0;
            byte[] buffer = new byte[512];
            bytes = client.EndReceive(ar);
            if(bytes > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytes));
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                if (state.sb.Length > 1)
                {
                    response = state.sb;
                    Console.Write("This is the response from the server: {0}", response.ToString());
                }
                receiveDone.Set();
            }
        }

        private Socket CreateSocket(IPAddress hostEntry)
        {
            IPEndPoint endPoint = new IPEndPoint(hostEntry, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socket);
            connectDone.WaitOne();
            return socket;
        }

        private void SendPackage(Socket socket, string message)
        {
            byte[] requestBytes = Encoding.ASCII.GetBytes(message);
            socket.BeginSend(requestBytes, 0, requestBytes.Length, 0, new AsyncCallback(SendCallback), socket);
            sendDone.WaitOne();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SynchronousClient sc = new SynchronousClient("45.55.77.201");
            Console.WriteLine("Type the message to send with a synchronous client:");

            string message = Console.ReadLine();
            string response = sc.SendMessage(message);

            Console.WriteLine();
            if (response != "")
            {
                Console.Write("This is the response from the server: {0}", response);
            }
            Console.ReadKey();

            AsynchronousClient asc = new AsynchronousClient("45.55.77.201");
            Console.WriteLine("Type the message to send with an asynchronous client:");

            message = Console.ReadLine();
            response = asc.SendMessage(message);

            Console.WriteLine();
            if (response != "")
            {
                Console.Write("This is the response from the server: {0}", response);
            }
            Console.ReadKey();
        }
    }
}
