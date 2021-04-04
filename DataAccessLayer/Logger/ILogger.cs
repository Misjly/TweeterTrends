namespace Tweets_Statistics.DataAccessLayer.Logger
{
    public interface ILogger<T>
    {
        void Log(T value);
    }
}
