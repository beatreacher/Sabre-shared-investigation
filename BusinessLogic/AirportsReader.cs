using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Model;

namespace BusinessLogic
{
    public class AirportsReader
    {
        private const string AirportsFilePath = "Resources/Airports.csv";
        public IEnumerable<Airport> Read()
        {
            using (TextReader fileReader = File.OpenText(AirportsFilePath))
            {
                using (var csv = new CsvReader(fileReader))
                {
                    csv.Configuration.PrepareHeaderForMatch = (s, i) => { return s.ToLower(); };

                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        yield return csv.GetRecord<Airport>();
                    }
                }                   
            }
        }
    }
}
