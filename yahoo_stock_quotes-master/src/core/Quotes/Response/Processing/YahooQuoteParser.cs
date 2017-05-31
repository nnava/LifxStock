using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YSQ.core.Quotes.Response.Processing
{
    internal class YahooQuoteParser : IParseAYahooQuote
    {
        public dynamic Parse(string quote_data, IEnumerable<QuoteReturnParameter> return_parameters)
        {
            using (var reader = new CsvReader(new StringReader(quote_data)))
            {
                var parsed_data = Parse(reader);
                return new YahooQuote(parsed_data.ToList(), return_parameters);
            }
        }

        IEnumerable<string> Parse(CsvReader reader)
        {
            reader.ReadHeader();
            return reader.FieldHeaders.ToList();
        }
    }
}