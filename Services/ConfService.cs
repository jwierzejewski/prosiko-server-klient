using Commons;
using Server.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class ConfService : IServiceModule
    {
        private List<IListener> listeners;
        private StartService startService;
        private RemoveService removeService;
        private StartedListeners startedListeners;
        private StartMedium startMedium;
        private StopMedium stopMedium;
        private StartedServices startedServices;



        public delegate void RemoveService(string serviceName);
        public delegate void StartService(string serviceName);
        public delegate string StartedListeners();
        public delegate string StartedServices();
        public delegate int StopMedium(string mediumName);
        public delegate void StartMedium(string Name,object address);

        public delegate void CommunicatorD(ICommunicator commander);

        public ConfService(StartedListeners startedListeners, StartedServices startedServices, StartMedium startMedium, StopMedium stopMedium, StartService startService, RemoveService removeService) {
            this.startedListeners = startedListeners;
            this.startedServices = startedServices;
            this.startMedium = startMedium;
            this.stopMedium = stopMedium;
            this.startService = startService;
            this.removeService = removeService;
        }

        public string AnswerCommand(string command)
        {
            string commandType = CommonTools.GetParam(command, 1);
            string answer = "";

            switch (commandType)
            {
                case "start-service":
                    answer = startServiceAnswer(command);
                    break;

                case "stop-service":
                    answer = stopServiceAnswer(command);
                    break;

                case "start-medium":
                    answer = startMediumAnswer(command);
                    break;
                case "stop-medium":
                    answer = stopMediumAnswer(command);
                    break;
                case "medium-list":
                    answer = startedMediumAnswer(command);
                    break;
                case "services-list":
                    answer = startedServicesAnswer(command);
                    break;
            }
            answer += "\n";

            return answer;
        }

        private string startedMediumAnswer(string command)
        {
            return "conf started-medium " + startedListeners();
        }

        private string startedServicesAnswer(string command)
        {
            return "conf started-services " + startedServices();
        }

        private string stopMediumAnswer(string command)
        {
            string mediumName = CommonTools.GetParam(command, 2);

            if(stopMedium(mediumName)>0)
            {
                return "conf stop-medium successfull";
            }
            return "conf stop-medium unsuccessfull";
        }

        private string startMediumAnswer(string command)
        {
            string mediumName = CommonTools.GetParam(command, 2);
            string mediumAdress = CommonTools.GetParam(command, 3);

            try
            {
                startMedium(mediumName, mediumAdress);
            }
            catch
            {
                return "conf start-medium unsuccessfull";
            }
            return "conf start-medium successfull";
            
        }

        private string stopServiceAnswer(string command)
        {
            string serviceName = CommonTools.GetParam(command, 2);
            try
            {
                removeService(serviceName);
            }
            catch 
            {
                return "conf stop-service unsuccessfull";
            }
            return "conf stop-service successfull";
        }

        private string startServiceAnswer(string command)
        {
            string serviceName = CommonTools.GetParam(command, 2);
            try
            {
                startService(serviceName);
            }
            catch
            {
                return "conf start-service unsuccessfull";
            }
            return "conf start-service successfull";
        }
    }
}
