using System;

namespace Tweets_Statistics.DataAccessLayer.Logger
{
    public class ConsoleLogger : ILogger<string>
    {
        public void Log(string value)
        {
            Console.WriteLine(value);
        }
    }
}
