using Tweets_Statistics.BusinessLayer;
using Tweets_Statistics.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Tweets_Statistics.PresentationLayer
{
    public class FormInitializer : IProcessInitializer
    {
        private readonly DataContext Context;

        public FormInitializer(DataContext dataContext)
        {
            Context = dataContext;
        }
        public void Start()
        {
            string fileName = ConfigurationManager.AppSettings["tweets"];
            string[] stringTweets = File.ReadAllText(fileName).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            IList<Tweet> tweets = new List<Tweet>();
            IParser<Tweet> parser = new TweetParser();

            foreach (string tweet in stringTweets)
            {
                tweets.Add(parser.Parse(tweet));
            }

            SentimentsParser.Parse(tweets, Context.Sentiments);

            StateFinder.Find(tweets, Context.StatesPolygons);

            Context.StatesSentiments = StatesEvaluator.Evaluate(tweets, Context.StatesPolygons.Keys);
        }
    }
}
