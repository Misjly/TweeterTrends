using Tweets_Statistics.BusinessLayer;
using System;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Text.RegularExpressions;
using Tweets_Statistics.DataAccessLayer.Logger;

namespace Tweets_Statistics.DataAccessLayer
{
    public class TweetParser : IParser<Tweet>
    {
        public Tweet Parse(string stringValue)
        {
            ILogger<string> logger = new WinLogger();
            (double, double) coordinates = (0, 0);
            DbGeography dbCoordinates = null;
            DateTime dateTime = new DateTime();
            string value = "";

            try
            {
                Regex regex = new Regex(@"\[(?<coordinate1>\-?\d+\.\d+),\s(?<coordinate2>\-?\d+\.\d+)\]\s_\s(?<year>[0-9]{4})\-(?<month>[0-9]{2})\-(?<day>[0-9]{2})\s(?<hour>[0-9]{2})\:(?<minute>[0-9]{2})\:(?<second>[0-9]{2})\s(?<text>[\w\W]+)");
                Match match = regex.Match(stringValue);
                coordinates.Item1 = double.Parse(match.Groups["coordinate1"].Value, CultureInfo.InvariantCulture);
                coordinates.Item2 = double.Parse(match.Groups["coordinate2"].Value, CultureInfo.InvariantCulture);

                dbCoordinates = DbGeography.PointFromText(string.Format($"POINT(" +
                coordinates.Item2.ToString(CultureInfo.InvariantCulture) + " " +
                coordinates.Item1.ToString(CultureInfo.InvariantCulture) + ")"), 4326);

                dateTime = new DateTime(
                    int.Parse(match.Groups["year"].Value),
                    int.Parse(match.Groups["month"].Value),
                    int.Parse(match.Groups["day"].Value),
                    int.Parse(match.Groups["hour"].Value),
                    int.Parse(match.Groups["minute"].Value),
                    int.Parse(match.Groups["second"].Value));

                value = match.Groups["text"].Value;
            }
            catch (Exception)
            {
                logger.Log($"String{Environment.NewLine}'{stringValue}'{Environment.NewLine}doesn't correspond parse form");
                Environment.Exit(0);
            }
            Tweet tweet = new Tweet(dbCoordinates, dateTime, value);

            return tweet;
        }
    }
}

