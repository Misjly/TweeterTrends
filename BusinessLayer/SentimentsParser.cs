using System.Collections.Generic;

namespace Tweets_Statistics.BusinessLayer
{
    public static class SentimentsParser
    {
        public static void Parse(IList<Tweet> tweets, IDictionary<string, float> sentiments)
        {
            int i = 0;
            foreach (var tweet in tweets)
            {
                int sentimentCount = 0;
                float tempSentiment = 0;
                foreach (var item in sentiments)
                {
                    if (tweet.Value.Contains(item.Key.ToLower()))
                    {
                        tempSentiment += item.Value;
                        sentimentCount++;
                        tweet.ContainsSentiment = true;
                    }
                }
                if (sentimentCount != 0)
                    tweet.Sentiment = tempSentiment / sentimentCount;

            }
        }
    }
}

