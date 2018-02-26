namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class Distance
    {
        public static (int distance, IList<int> nodes) CalcEccentricity(IList<int[]> graph, int node)
        {
            var passedNodes = new bool[graph.Count];

            passedNodes[node] = true;

            var front = new List<int> { node };

            int e = 0;
            for (; ; e++)
            {
                var newFront = new List<int>();
                foreach (var frontNode in front)
                {
                    foreach (var siblingNode in graph[frontNode])
                    {
                        if (!passedNodes[siblingNode])
                        {
                            passedNodes[siblingNode] = true;
                            newFront.Add(siblingNode);
                        }
                    }
                }

                if (newFront.Count == 0)
                {
                    return (e, front);
                }

                front = newFront;
            }
        }

        public static (int diameter, IEnumerable<(int, int)> pairs) CalcDiameter(IList<int[]> graph)
        {
            var pairs = new List<(int, int)>();
            var maxEccentricity = 0;

            for (var i = 0; i < graph.Count; i++)
            {
                var (eccentricity, nodes) = CalcEccentricity(graph, i);
                if (eccentricity >= maxEccentricity)
                {
                    if (eccentricity > maxEccentricity)
                    {
                        maxEccentricity = eccentricity;
                        pairs.Clear();
                    }

                    pairs.AddRange(nodes.Select(n => (i, n)));
                }
            }

            return (maxEccentricity, pairs);
        }
    }
}