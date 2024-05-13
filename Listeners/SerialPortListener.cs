using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Communicators;

namespace Server.Listeners
{
    internal class SerialPortListener : IListener
    {
        private string portName;
        private SerialPort serialPort;

        public SerialPortListener(object portName = null)
        {
            if (portName == null)
                this.portName = "COM1";
            else
                this.portName = (string)portName;
            serialPort = new SerialPort(this.portName);
        }

        public string Name => $"SerialPort:{portName}";

        public void Start(CommunicatorD onConnect)
        {
            SerialPortCommunicator SPCommunicator = new SerialPortCommunicator(serialPort);
            onConnect(SPCommunicator);
        }

        public void Stop()
        {
            serialPort.Close();
        }
    }
}
