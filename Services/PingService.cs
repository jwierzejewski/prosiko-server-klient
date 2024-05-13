using Commons;


namespace Server.Services
{
    internal class PingService : IServiceModule
    {

        public string AnswerCommand(string command)
        {
            var length = GetAnswerLength(command);
            return "ping " + CommonTools.Trush(length - 6) + '\n';
        }

        int GetAnswerLength(string command)
        {
            int indexStart = command.IndexOf(" ");
            int indexEnd = command.IndexOf(" ", indexStart + 1);
            string lengthText = command.Substring(indexStart + 1, indexEnd - indexStart - 1);
            return int.Parse(lengthText);
        }
    }
}