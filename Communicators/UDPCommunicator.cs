using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Communicators
{
    //max 64kB
    internal class UDPCommunicator : ICommunicator
    {
        private int portNo;
        private UdpClient udpClient;
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private Thread _thread;
        private bool shouldTerminate = false;

        public UDPCommunicator(int portNo) 
        {
            this.portNo = portNo;
        }


        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            _thread = new Thread(Communicate);
            _thread.Start();

            
        }

        private void Communicate()
        {
            UdpClient udpClient = new UdpClient(this.portNo);
            while(!shouldTerminate)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                string message = udpClient.RecievePakcges(ref RemoteIpEndPoint);

                Console.WriteLine($"R: {message.Length} B {message.SubstringMax(40)}");
                
                string answer = onCommand(message);
                
                Console.WriteLine($"S: {answer.Length} B {answer.SubstringMax(40)}");

                udpClient.SendPackages(answer, RemoteIpEndPoint);
                
            }
        }

        public void Stop()
        {
            shouldTerminate = true;
            if (udpClient != null)
                udpClient.Close();
        }

        public List<byte[]> createPackages(byte[] fullData)
        {
            int packageSize = 60000;
            int dataStart = 0;
            List<byte[]> packageList = new();
            while (dataStart < fullData.Length)
            {
                byte[] package = fullData.Skip(dataStart).Take(Math.Min(dataStart + packageSize, fullData.Length)).ToArray();
                packageList.Add(package);
                dataStart += packageSize;
            }
            return packageList;
        }
    }
}
