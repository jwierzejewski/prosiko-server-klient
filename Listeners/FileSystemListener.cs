using Server.Communicators;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Server.Listeners
{
    internal class FileSystemListener:IListener
    {
        string repoDirectoryPath;
        private FileSystemCommunicator FSCommunicator;

        public FileSystemListener()
        {
           this.repoDirectoryPath = "D:\\jacek\\jacek\\Prosiko\\FileSystemRepo";
        }

        public FileSystemListener(object repoDirectoryPath = null)
        {
            if (repoDirectoryPath == null)
                this.repoDirectoryPath = "D:\\jacek\\jacek\\Prosiko\\FileSystemRepo";
            else
                this.repoDirectoryPath = (string)repoDirectoryPath;
        }

        public string Name => $"FileSystem:{repoDirectoryPath}";

        public void Start(CommunicatorD onConnect)
        {
            bool exists = Directory.Exists(repoDirectoryPath);
            if (!exists)
                Directory.CreateDirectory(repoDirectoryPath);

            FSCommunicator = new FileSystemCommunicator(repoDirectoryPath);
            onConnect(FSCommunicator);
        }

        public void Stop()
        {
            FSCommunicator.Stop();
        }
    }
}
