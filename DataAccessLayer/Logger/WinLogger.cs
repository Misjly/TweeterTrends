using System.Windows.Forms;

namespace Tweets_Statistics.DataAccessLayer.Logger
{
    public class WinLogger : ILogger<string>
    {
        public void Log(string value)
        {
            MessageBox.Show(value);
        }
    }
}
