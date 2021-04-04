using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;

namespace Tweets_Statistics.DataAccessLayer
{
    public class DataContext
    {
        public IDictionary<string, IList<DbGeography>> StatesPolygons { get; set; }
        public IDictionary<string, IList<IList<PointF>>> StatesPolygonsPoints { get; set; }
        public IDictionary<string, float> Sentiments { get; set; }
        public string MapSource { get; set; }
        public IDictionary<string, float> StatesSentiments { get; set; }

        public DataContext()
        {
            StatesPolygons = new Dictionary<string, IList<DbGeography>>();
            StatesPolygonsPoints = new Dictionary<string, IList<IList<PointF>>>();
            Sentiments = new Dictionary<string, float>();
        }
    }
}
