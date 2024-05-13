using Server.Communicators;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Listeners
{
    internal class UDPListener : IListener
    {
        private int portNo;
        private UDPCommunicator udpCommunicator;

        public UDPListener(object portNo) 
        {
            if (portNo == null)
                this.portNo = 12346;
            else
                this.portNo = int.Parse(portNo.ToString());
        }

        public string Name => $"UDP:{portNo}";

        public void Start(CommunicatorD onConnect)
        {
            udpCommunicator = new UDPCommunicator(this.portNo);
            onConnect(udpCommunicator);
        }

        public void Stop()
        {
            udpCommunicator.Stop();
        }
    }
}
