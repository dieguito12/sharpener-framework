using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SharpenerClasses
{
    public class WebServer
    {
        private static TcpListener myListener;
        private static int port = 8084;
        private static IPAddress host = IPAddress.Parse("0.0.0.0");

        public static void StartListen()
        {
            myListener = new TcpListener(host, port);
            myListener.Start();

            while (true)
            {
                //Accept a new connection
                Socket socket = myListener.AcceptSocket();

                Console.WriteLine("Socket Type " + socket.SocketType);
                if (socket.Connected)
                {
                    Console.WriteLine("\nClient Connected!! Client IP {0}\n", socket.RemoteEndPoint);
                    byte[] toBytes = Encoding.ASCII.GetBytes("Connected!!");
                    socket.Send(toBytes);

                    Byte[] bReceive = new Byte[1024];
                    int i = socket.Receive(bReceive, bReceive.Length, 0);

                    //Convert Byte to String
                    string sBuffer = Encoding.ASCII.GetString(bReceive);
                    Console.WriteLine("Http Message from {0}:\n{1}", socket.RemoteEndPoint, sBuffer);
                }
                socket.Close();
            }
        }
    }
}
