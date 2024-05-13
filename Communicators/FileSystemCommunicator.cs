using Commons;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communicators
{
    internal class FileSystemCommunicator : ICommunicator
    {
        private string repoDirectoryPath;
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private Thread _thread;
        FileSystemWatcher watcher;

        public FileSystemCommunicator(string repoDirectoryPath) 
        {
            this.repoDirectoryPath = repoDirectoryPath;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            _thread = new Thread(Communicate);
            _thread.Start();
        }

        private void Communicate(object? obj)
        {
            watcher = new FileSystemWatcher(repoDirectoryPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Filter = "*.in";
            watcher.Changed += onChanged;
            watcher.Deleted += onDeleted;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void onDeleted(object sender, FileSystemEventArgs e)
        {
            File.Delete(CreateReponseFilePath(e.FullPath));
        }

        private void onChanged(object sender, FileSystemEventArgs e)
        {
            string line = File.ReadLines(e.FullPath).Last();

            Console.WriteLine($"R: {line.Length} B {line.SubstringMax(40)}");
            string answer = this.onCommand(line);
            Console.WriteLine($"S: {answer.Length} B {answer.SubstringMax(40)}");
            File.AppendAllText(CreateReponseFilePath(e.FullPath), answer);
        }

        private string CreateReponseFilePath(string fullPath)
        {
            return Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath)+".out");
        }

        public void Stop()
        {
            watcher.Dispose();
        }
    }
}
