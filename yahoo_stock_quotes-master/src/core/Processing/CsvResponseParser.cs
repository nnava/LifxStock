using CsvHelper;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using YSQ.core.Extensions;

namespace YSQ.core.Processing
{
    internal class CsvResponseParser : IParseACsvResponse
    {
        public IEnumerable<string> ParseToLines(WebResponse response)
        {
            using (var response_stream = new StreamReader(response.GetResponseStream()))
            {
                while (!response_stream.EndOfStream)
                    yield return response_stream.ReadLine();
            }
        }

        public virtual ParsedCSVFile ParseToRows(WebResponse response)
        {
            using (var reader = new CsvReader(new StreamReader(response.GetResponseStream())))
            {
                return new ParsedCSVFile(GetRows(reader).ToArray(), reader.FieldHeaders.Select(StringExtensions.RemoveWhitespace).ToArray());
            }
        }

        IEnumerable<dynamic> GetRows(CsvReader reader)
        {
            var fieldCount = reader.FieldHeaders.Count();
            var headers = reader.FieldHeaders;

            while (reader.ReadHeader())
            {
                IDictionary<string, object> row = new ExpandoObject();
                for (var i = 0; i < fieldCount; i++)
                    row.Add(headers[i].RemoveWhitespace(), reader[i]);
                yield return row;
            }
        }
    }
}