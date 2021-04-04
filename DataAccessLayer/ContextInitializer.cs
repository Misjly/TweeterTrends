using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Tweets_Statistics.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tweets_Statistics.DataAccessLayer.Logger;
using System.Drawing;

namespace Tweets_Statistics.DataAccessLayer
{
    class ContextInitializer
    {
        public DataContext Context { get; }

        public ContextInitializer()
        {
            ILogger<string> logger = new WinLogger();
            Context = new DataContext();
            string sentimentsFilename = "";
            string polygonsFilename = "";

            try
            {
                Context.MapSource = ConfigurationManager.AppSettings["map"];
                using (var reader = new StreamReader(ConfigurationManager.AppSettings["sentiments"]))
                {
                    using (var csvReader =
                        new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HasHeaderRecord = false,
                            HeaderValidated = null,
                        }))
                    {

                        var records = csvReader.GetRecords<Tuple<string, float>>();

                        foreach (var record in records)
                        {
                            Context.Sentiments.Add(record.Item1, record.Item2);
                        }
                    }
                }
            }
            catch (System.Configuration.ConfigurationException)
            {
                logger.Log($"Error in Configuration file");
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                logger.Log($"File {sentimentsFilename} wasn't found");
                Environment.Exit(0);
            }
            catch (Exception)
            {
                logger.Log($"{sentimentsFilename} file value doesn't correspond parse form");
                Environment.Exit(0);
            }



            try
            {
                polygonsFilename = ConfigurationManager.AppSettings["states"];
                string text = File.ReadAllText(polygonsFilename);
                char[] spaces = new char[] { ' ', '\t', '\n', '\r' };
                int index;

                do
                {
                    index = text.IndexOfAny(spaces);
                    if (index != -1)
                        text = text.Remove(index, 1);
                } while (index != -1);

                Regex regex = new Regex("(?=\"(?<postalCode>[A-Z]{2})\":(?<polygons>\\[.+?\\])(,\"|\\}))");
                List<Match> matches = regex.Matches(text).Cast<Match>().ToList();

                foreach (var match in matches)
                {
                    string tempString = match.Groups["polygons"].Value;
                    tempString = tempString.Replace("[[[", "[[");
                    tempString = tempString.Replace("]]]", "]]");
                    IList<IList<IList<float>>> polygonsCoordinates = new List<IList<IList<float>>>();
                    if (tempString[2] == '[')
                    {
                        polygonsCoordinates = JsonConvert.DeserializeObject<List<IList<IList<float>>>>(tempString);
                    }
                    else
                    {
                        polygonsCoordinates.Add(JsonConvert.DeserializeObject<List<IList<float>>>(tempString));
                    }

                    IList<DbGeography> polygons = new List<DbGeography>();
                    IList<IList<PointF>> polygonsPoints = new List<IList<PointF>>();

                    foreach (var polygon in polygonsCoordinates)
                    {
                        polygons.Add(GeographyConverter.ConvertCoordinatesToPolygon(polygon));
                        polygonsPoints.Add(GeographyConverter.ConvertCoordinatesToPoints(polygon));
                    }
                    Context.StatesPolygons.Add(match.Groups["postalCode"].Value, polygons);
                    Context.StatesPolygonsPoints.Add(match.Groups["postalCode"].Value, polygonsPoints);
                }
            }
            catch (System.Configuration.ConfigurationException)
            {
                logger.Log($"Error in Configuration file");
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                logger.Log($"File {polygonsFilename} wasn't found");
                Environment.Exit(0);
            }
            catch (Exception)
            {
                logger.Log($"File {polygonsFilename} doesn't correspond parse form");
                Environment.Exit(0);
            }
        }
    }
}

