using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;

namespace Tweets_Statistics.BusinessLayer
{
    public static class StateFinder
    {
        private static bool IsInside(DbGeography polygon, DbGeography point)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            var wellKnownText = polygon.AsText();

            var sqlGeography =
                SqlGeography.STGeomFromText(new SqlChars(wellKnownText), DbGeography.DefaultCoordinateSystemId)
                    .MakeValid();

            var invertedSqlGeography = sqlGeography.ReorientObject();

            if (sqlGeography.STArea() > invertedSqlGeography.STArea())
            {
                sqlGeography = invertedSqlGeography;
            }

            polygon = DbSpatialServices.Default.GeographyFromProviderValue(sqlGeography);

            return point.Intersects(polygon);
        }

        public static void Find(IList<Tweet> tweets, IDictionary<string, IList<DbGeography>> states)
        {
            bool isFound;
            foreach (var tweet in tweets)
            {
                isFound = false;
                foreach (var state in states)
                {
                    foreach (var polygon in state.Value)
                    {
                        if (IsInside(polygon, tweet.Coordinates))
                        {
                            tweet.State = state.Key;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                        break;
                }
            }

        }
    }
}
