namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;


    public class Program
    {
        public static void Main(string[] args)
        {
            var graph = LoadGraph("../");
        }

        public static GraphInfo LoadGraph(string inputPath)
        {
            return JsonConvert.DeserializeObject<GraphInfo>(File.ReadAllText(inputPath));
        }

        public class GraphInfo
        {
            public Dictionary<string, int[]> Edges;

            public Dictionary<string, int> Nodes;
        }
    }
}
