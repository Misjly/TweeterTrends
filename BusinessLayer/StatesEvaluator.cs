using System.Collections.Generic;
using System.Linq;

namespace Tweets_Statistics.BusinessLayer
{
    public static class StatesEvaluator
    {
        public static IDictionary<string, float> Evaluate(IList<Tweet> tweets, ICollection<string> states)
        {
            IDictionary<string, float> keyValues = new Dictionary<string, float>();
            foreach (var state in states)
            {
                float value = 0;
                int count = 0;
                bool hasSentiment = false;
                foreach (var tweet in tweets.Where(x => x.State == state))
                {
                    if (tweet.ContainsSentiment)
                    {
                        count++;
                        value += tweet.Sentiment;
                        hasSentiment = true;
                    }
                }
                if (!hasSentiment)
                    value = -2;
                else
                {
                    if (count > 0)
                        value /= count;
                }
                keyValues.Add(state, value);
            }
            return keyValues;
        }
    }
}
