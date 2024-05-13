
using Server;
using Server.Listeners;
using Server.Services;


Serwer serwer = new Serwer();

serwer.AddServiceModule("ping", new PingService());
serwer.AddServiceModule("file", new FileService());
serwer.AddServiceModule("chat", new ChatService());
serwer.AddListener<TCPListener>(true,12345);
serwer.AddListener<SerialPortListener>(true,"COM1");
serwer.AddListener<FileSystemListener>(true);
serwer.AddListener<UDPListener>(true,12345);
serwer.Start();