using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tweets_Statistics.BusinessLayer
{
	public static class GeographyConverter
	{
		public static DbGeography ConvertCoordinatesToPolygon(IEnumerable<IList<float>> coordinates)
		{
			var coordinateList = coordinates.ToList();
			double firstResult = Math.Abs(coordinateList.First()[0] - coordinateList.Last()[0]);
			double lastResult = Math.Abs(coordinateList.First()[1] - coordinateList.Last()[1]);
			double tolerance = 0.000001;

			if (firstResult > tolerance || lastResult > tolerance)
			{
				throw new Exception("First and last point do not match. This is not a valid polygon");
			}

			var count = 0;
			var sb = new StringBuilder();
			sb.Append(@"POLYGON((");
			foreach (var coordinate in coordinateList)
			{
				if (count == 0)
				{
					sb.Append(coordinate[0].ToString(CultureInfo.InvariantCulture) + " " + coordinate[1].ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					sb.Append("," + coordinate[0].ToString(CultureInfo.InvariantCulture) + " " + coordinate[1].ToString(CultureInfo.InvariantCulture));
				}

				count++;
			}

			sb.Append(@"))");

			return DbGeography.PolygonFromText(sb.ToString(), 4326);
		}

		public static IList<PointF> ConvertCoordinatesToPoints(IList<IList<float>> coordinates)
		{
			var coordinateList = coordinates.ToList();
			var points = new List<PointF>();
			const int width = 1920;
			const int height = 1080;


			foreach (var point in coordinateList)
			{
				double latRad = (point.Last() * Math.PI) / 180;

				float temp = (float)Math.Log(Math.Tan((Math.PI / 4) + (latRad / 2)));
				
				float posX = (point.First() + 180) * (width / 180);

				float posY = (float)((height / 2) - (width * temp / (2 * Math.PI)));

				points.Add(new PointF(posX, posY));
			}
			return points;
		}
	}
}
