using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Serwer
    {
        Dictionary<string, IServiceModule> accessibleServices = new();
        Dictionary<string, IServiceModule> startedServices = new();
        public List<Type> listeners = new();
        public Dictionary<string, IListener> startedListeners = new();
        List<ICommunicator> communicators = new();

        public Serwer()
        {
            startedServices.Add("conf", new ConfService(ListenersList,ServicesList, StartListener, StopListener, StartServiceModule, StopServiceModule));
        }

        public void AddServiceModule(string name, IServiceModule service)
        {
            accessibleServices.Add(name, service);
        }

        public void StartServiceModule(string name)
        {
            startedServices.Add(name, accessibleServices[name]);
        }

        public void StopServiceModule(string name)
        {
            if (accessibleServices.ContainsKey(name))
                startedServices.Remove(name);
        }

        public void AddCommunicator(ICommunicator communicator)
        {
            communicators.Add(communicator);
            communicator.Start(CentrumUsług, DisconnectCommunicator);
        }

        private void DisconnectCommunicator(ICommunicator commander)
        {
            communicators.Remove(commander);
        }

        private string CentrumUsług(string command)
        {
            try
            {
                var commandType = GetCommandType(command);
                var service = startedServices[commandType];

                return service.AnswerCommand(command);
            }
            catch (Exception ex)
            {
                return $"Service failure with exception {ex.Message}";
            }
        }

        private string GetCommandType(string command)
        {
            var separator = " ";
            var index = command.IndexOf(separator);
            return command.Substring(0, index);
        }

        public void AddListener<IListenerClass>(bool startListener = false, object address = null) where IListenerClass : IListener
        {
            listeners.Add(typeof(IListenerClass));
            IListener listener;
            if (startListener)
            {
                if (address != null)
                {
                    object[] constructorArgs = new object[] { address };
                    listener = Activator.CreateInstance(typeof(IListenerClass), constructorArgs) as IListener;
                }
                else
                {
                    listener = Activator.CreateInstance(typeof(IListenerClass)) as IListener;
                }
                startedListeners.Add(listener.Name, listener);
            }
        }

        public string ListenersList()
        {
            return string.Join(", ", startedListeners.Keys);
        }

        public string ServicesList()
        {
            return string.Join(", ", startedServices.Keys);
        }

        public int StopListener(string listenerName)
        {
            int counter = 0;
            foreach (IListener listener in listeners)
            {
                if (listener.Name == listenerName)
                {
                    counter++;
                    listener.Stop();
                }
            }
            return counter;
        }

        public void StartListener(string Name, object address)
        {
            foreach (var listenerType in listeners)
            {
                if (listenerType.Name.Equals(Name + "listener", StringComparison.OrdinalIgnoreCase) )
                {
                    object[] parameter = { address };
                    var listener = Activator.CreateInstance(listenerType, parameter) as IListener;
                    listener.Start(AddCommunicator);
                    startedListeners.Add(listener.Name, listener);
                }
                else
                {
                    throw new Exception("Listener do not exists");
                }
            }
        }
        internal void Start()
        {
            foreach (var listener in startedListeners)
            {
                listener.Value.Start(AddCommunicator);
            }
        }
    }
}
