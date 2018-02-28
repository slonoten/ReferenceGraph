namespace ReferenceGraphBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    internal class ReferenceGrabber
    {
        private readonly List<ConcurrentBag<string>> urlQueue;

        private readonly ConcurrentDictionary<string, object> urlDictionary = new ConcurrentDictionary<string, object>();

        private readonly Regex includeRegex;

        private readonly Regex excludeRegex;

        private readonly Uri rootUri;

        public ReferenceGrabber(string rootUrl, int depth, string includeFilter, string excludeFilter)
        {
            if (depth < 1)
            {
                throw new ArgumentException("Grab depth must be greater then 0", "depth");
            }

            this.rootUri = new Uri(rootUrl);

            if (!this.rootUri.IsAbsoluteUri)
            {
                throw new ArgumentException("Root URL should be absolute", "rootUrl");
            }

            this.urlQueue = new List<ConcurrentBag<string>>(Enumerable.Range(0, depth).Select(x => new ConcurrentBag<string>()));
            this.urlQueue[0].Add(rootUrl);

            this.includeRegex = new Regex(includeFilter, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.excludeRegex = new Regex(excludeFilter, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void Grab(ConcurrentQueue<(string url, IEnumerable<string> references)> queue)
        {
            for (var i = 0; i < this.urlQueue.Count; i++)
            {
                var level = this.urlQueue[i];

                Parallel.ForEach(
                    level, 
                    new ParallelOptions { MaxDegreeOfParallelism = 64 },
                    url => 
                    {
                                var references = ExtractReferences(url)
                                    .Where(u => this.includeRegex.IsMatch(u) && !this.excludeRegex.IsMatch(u))
                                    .Select(this.NormalizeUrl)
                                    .Distinct()
                                    .ToArray();

                                queue.Enqueue((url, references));
                                EnqueueReferences(references, i + 1);
                    }
                );
            }
        }

        private static IEnumerable<string> ExtractReferences(string url)
        {
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(url);

            var references = htmlDoc.DocumentNode.SelectNodes("//descendant::a/@href").Select(n => n.Attributes["href"].Value);

            return references;
        }

        private void EnqueueReferences(IEnumerable<string> references, int level)
        {
            if (level >= this.urlQueue.Count)
            {
                return; 
            }

            var levelQueue = this.urlQueue[level];

            foreach(var url in references.Where(url => this.urlDictionary.TryAdd(url, null)))
            {
                levelQueue.Add(url);
            }
            
        }

        private string NormalizeUrl(string url)
        {
            var uri = new Uri(url.ToLower(), UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsoluteUri;
            }

            return new Uri(this.rootUri, uri).AbsoluteUri;
        }
    }
}