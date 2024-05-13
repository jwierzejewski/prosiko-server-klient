using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Commons;

namespace Server
{
    internal class SerialPortCommunicator : ICommunicator
    {
        private string portName;
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private Thread _thread;
        private SerialPort _serialPort;
        private bool shouldTerminate = false;

        public SerialPortCommunicator(SerialPort serialPort) 
        {
            this._serialPort = serialPort;
            _serialPort.ReadTimeout = 50000;
            _serialPort.WriteTimeout = 50000;
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
            string name;
            string message;

            _serialPort.Open();

            while (!shouldTerminate)
            {
                try
                {
                    message = _serialPort.ReadLine();
                    Console.WriteLine($"R: {message.Length} B {message.SubstringMax(40)}");

                    string answer = this.onCommand(message);
                    Console.WriteLine($"S: {answer.Length} B {answer.SubstringMax(40)}");
                    _serialPort.Write(answer);
                }
                catch (TimeoutException) { }
            }

            _serialPort.Close();
        }

        public void Stop()
        {
            shouldTerminate = false;
            _serialPort.Close();
        }
    }
}
