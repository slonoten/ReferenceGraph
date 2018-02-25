namespace ReferenceGraphBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Xml;
    using System.IO;

    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    public class Program
    {    
        public static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

                string url, includeFilter, excludeFilter;
                int depth;
                try
                {
                    var configuration = builder.Build();
                    url = configuration["url"];
                    includeFilter = configuration["include_regex"];
                    excludeFilter = configuration["exclude_regex"];
                    depth = int.Parse(configuration["depth"]);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException("Error reading configuration", ex);
                }

                var grabber = new ReferenceGrabber(url, depth, includeFilter, excludeFilter);

                SaveGraph(grabber.Grab(), "graph.json");

                /*foreach(var res in grabber.Grab())
                {
                    Console.WriteLine(res.url);
                    foreach (var reference in res.references)
                    {
                        Console.WriteLine("\t" + reference);
                    }
                }*/
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void SaveGraph(IEnumerable<(string url, IEnumerable<string> refs)> pagesReferences, string outPath)
        {
            var urlIds = new Dictionary<string, int>();

            using(var writer = new StreamWriter(outPath, false))
            {
                using(var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("edges");
                    jsonWriter.WriteStartObject();
                    foreach(var pageReferences in pagesReferences)
                    {
                        jsonWriter.WritePropertyName(GetId(pageReferences.url).ToString());
                        jsonWriter.WriteStartArray();
                        foreach(var reference in pageReferences.refs)
                        {
                            jsonWriter.WriteValue(GetId(reference));
                        }
                        jsonWriter.WriteEndArray();
                    }
                    jsonWriter.WriteEndObject();
                    jsonWriter.WritePropertyName("nodes");
                    jsonWriter.WriteStartArray();
                    foreach(var urlIdPair in urlIds)
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WritePropertyName(urlIdPair.Key);
                        jsonWriter.WriteValue(urlIdPair.Value);
                        jsonWriter.WriteEndObject();
                    }
                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                }
            }

            int GetId(string url)
            {
                if (urlIds.TryGetValue(url, out var id))
                {
                    return id;
                }

                var newId = urlIds.Count;
                urlIds.Add(url, newId);
                return newId;
            }
        }
    }
}
