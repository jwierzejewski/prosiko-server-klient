
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.CompilerServices;
using Server.Communicators;

namespace Server.Listeners
{
    internal class TCPListener : IListener
    {
        Thread _thread;
        int portNo;
        CommunicatorD onConnect;
        TcpListener server;
        private bool shouldTermiante;

        public TCPListener(object portNo)
        {
            if (portNo == null)
                this.portNo = 12345;
            else
                this.portNo = int.Parse(portNo.ToString());
        }

        public string Name => $"TCP:{portNo}";

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect = onConnect;
            shouldTermiante = false;
            _thread = new Thread(Listen);
            _thread.Start();
        }

        private void Listen()
        {
            server = new TcpListener(IPAddress.Any, portNo);
            server.Start();
            while (!shouldTermiante)
            {
                TcpClient client = server.AcceptTcpClient();
                if (client != null)
                {
                    Console.WriteLine($"TCP connect: {client.Client.RemoteEndPoint}");
                    TCPCommunicator tCPCommunicator = new TCPCommunicator(client);
                    onConnect(tCPCommunicator);
                }
            }
        }

        public void Stop()
        {
            shouldTermiante = true;
            server.Stop();
        }
    }
}