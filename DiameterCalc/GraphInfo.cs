namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    public class GraphInfo
    {
        public Dictionary<string, int[]> Edges;

        public Dictionary<string, int> Nodes;

        public static GraphInfo Load(string inputPath)
        {
            return JsonConvert.DeserializeObject<GraphInfo>(File.ReadAllText(inputPath));
        }

        public IList<int[]> BuildSparseIncedenceMatrix()
        {
            var maxIndex = -1;
            var matrix = new List<int[]>(this.Edges.Count);
            foreach (var nodeEdges in this.Edges)
            {
                var node = int.Parse(nodeEdges.Key);
                if (node >= matrix.Count)
                {
                    matrix.AddRange(Enumerable.Repeat(new int[0], node - matrix.Count + 1));
                }

                matrix[node] = nodeEdges.Value;
                var nodeMaxIndex = nodeEdges.Value.Max();
                if (nodeMaxIndex > maxIndex)
                {
                    maxIndex = nodeMaxIndex;
                }
            }

            matrix.AddRange(Enumerable.Repeat(new int[0], maxIndex - matrix.Count + 1));

            return matrix;
        }
    }
}