using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communicators
{
    internal class TCPCommunicator : ICommunicator
    {

        TcpClient client;
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private Thread _thread;

        public TCPCommunicator(TcpClient client)
        {
            this.client = client;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            _thread = new Thread(Communicate);
            _thread.Start();
        }

        public void Stop()
        {
            if (client.Connected)
            {
                client.Close();
            }
        }

        void Communicate()
        {
            byte[] bytes = new byte[4096];
            string? data = null;
            int len, nl;
            NetworkStream stream = client.GetStream();
            try
            {
                while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    Console.WriteLine($"R: {len} B");
                    data += Encoding.UTF8.GetString(bytes, 0, len);
                    while ((nl = data.IndexOf('\n')) != -1)
                    {
                        string line = data.Substring(0, nl + 1);
                        data = data.Substring(nl + 1);
                        Console.WriteLine($"R: {line.Length} B {line.SubstringMax(40)}");
                        string answer = onCommand(line);
                        Console.WriteLine($"S: {answer.Length} B {answer.SubstringMax(40)}");
                        byte[] msg = Encoding.UTF8.GetBytes(answer);
                        stream.Write(msg, 0, msg.Length);
                    }
                }
            }
            catch { }
            if (client.Connected)
            {
                client.Close();
                onDisconnect(this);
            }
        }
    }
}
