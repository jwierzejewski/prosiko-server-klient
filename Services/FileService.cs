using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class FileService : IServiceModule
    {
        string directoryPath = ".\\FileService";


        public string AnswerCommand(string command)
        {
            Directory.CreateDirectory(directoryPath);
            string commandType = CommonTools.GetParam(command, 1);
            string answer = "";

            switch (commandType)
            {
                case "put":
                    answer = putAnswer(command);
                    break;

                case "get":
                    answer = getAnswer(command);
                    break;

                case "dir":
                    answer = dirAnswer();
                    break;
            }
            answer += "\n";

            return answer;
        }

        private string dirAnswer()
        {
            string answer;
            
            string[] fileNames = Directory.GetFiles(directoryPath);
            string answerData = string.Join(" ", fileNames);
            string answerDataEncoded = CommonTools.EncodeToBase64(answerData);

            answer = $"file dir {answerDataEncoded}";
            return answer;
        }

        private string putAnswer(string command)
        {
            string answer;
            string fileNameEncoded = CommonTools.GetParam(command, 2);
            string fileData = CommonTools.GetParam(command, 3);

            string fileName = CommonTools.DecodeFromBase64(fileNameEncoded);
            byte[] bytes = Convert.FromBase64String(fileData);

            File.WriteAllBytes(CommonTools.CreateFilePath(directoryPath, fileName), bytes);

            answer = "file put successful";
            return answer;
        }

        private string getAnswer(string command)
        {
            string answer;
            string fileNameEncoded = CommonTools.GetParam(command, 2);
            string fileName = CommonTools.DecodeFromBase64(fileNameEncoded);
            byte[] bytes = File.ReadAllBytes(CommonTools.CreateFilePath(directoryPath, fileName));
            string fileData = Convert.ToBase64String(bytes);
            answer = $"file get {fileNameEncoded} {fileData}";

            return answer;
        }
    }
}
