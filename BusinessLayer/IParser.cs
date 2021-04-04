namespace Tweets_Statistics.BusinessLayer
{
    public interface IParser<T>
    {
        T Parse(string stringValue);
    }
}
