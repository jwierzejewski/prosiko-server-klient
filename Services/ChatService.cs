using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Commons;

namespace Server.Services
{
    //Base64
    internal class ChatService : IServiceModule
    {
        private Dictionary<string, List<(string sender, string content)>> messeges;
        private HashSet<string> users;

        public ChatService()
        {
            messeges = new();
            users = new();
        }


        public string AnswerCommand(string command)
        {
            string serviceName = CommonTools.GetParam(command, 0);
            string commandType = CommonTools.GetParam(command, 1);
            string data = CommonTools.GetParam(command, 2);
            string decodedData = CommonTools.DecodeFromBase64(data);

            string answer = $"{serviceName} {commandType}";
            string answerData ="";

            switch (commandType)
            {
                case "msg":
                    answerData = msgHandler(decodedData);
                    break;

                case "get":
                    answerData = getHandler(decodedData);
                    break;

                case "who":
                    answerData = $"{string.Join(" ", users)}";
                    break;

                default:
                    break;
            }
            string encodedData = CommonTools.EncodeToBase64(answerData);


            answer += " "+encodedData+"\n";
            return answer;
        }

        private string getHandler(string data)
        {
            string reciever = CommonTools.GetParam(data, 0);
            string answer = $"{messagesList(reciever)}";
            return answer;
        }

        private string msgHandler(string data)
        {
            string answer;
            try
            {
                string reciever = CommonTools.GetParam(data, 0);
                string sender = CommonTools.GetParam(data, 1);
                string msg = CommonTools.FromSpecifiedDelimeterToEnd(data, 2);

                users.Add(reciever);
                users.Add(sender);

                if (messeges.ContainsKey(reciever))
                {
                    messeges[reciever].Add((sender, msg));
                }
                else
                {
                    messeges[reciever] = new List<(string, string)>() { (sender, msg) };
                }

                answer = "sended";
            }
            catch
            {
                answer = "sending_failed";
            }

            return answer;
        }

        private string messagesList(string reciever)
        {
            string messagesListString = "";
            if (messeges.ContainsKey(reciever))
            {
                foreach (var message in messeges[reciever])
                {
                    string messageString = $" from:{message.sender} content:{message.content}";
                    messagesListString += messageString;
                }
            }
            return messagesListString;
        }
    }
}
