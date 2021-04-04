using System;
using System.Data.Entity.Spatial;
namespace Tweets_Statistics.BusinessLayer
{
    public class Tweet
    {
        public DbGeography Coordinates { get; }
        public DateTime TimeDate { get; }
        public string Value { get; }
        public float Sentiment { get; set; }
        public string State { get; set; }
        public bool ContainsSentiment { get; set; }

        public Tweet(DbGeography coordinates, DateTime dateTime, string value)
        {
            Coordinates = coordinates;
            TimeDate = dateTime;
            Value = value.ToLower();
            ContainsSentiment = false;
        }
    }
}

