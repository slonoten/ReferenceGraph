namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Threading;    
    using System.Threading.Tasks;

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

        public static (int diameter, IEnumerable<(int, int)> pairs) CalcDiameterParallel(IList<int[]> graph)
        {
            var pairs = new ConcurrentQueue<(int leftNode, IList<int> rightNodes)>();
            var maxEccentricity = 0;
            var pairsLock = new object();

            Parallel.ForEach(Enumerable.Range(0, graph.Count),
                i =>
                {
                    var (eccentricity, nodes) = CalcEccentricity(graph, i);
                    // Check without lock
                    if (eccentricity >= maxEccentricity)
                    {
                        lock(pairsLock)
                        {
                            // Check again becouse max might be changed before we've got inside lock
                            if (eccentricity >= maxEccentricity)
                            {
                                if (eccentricity > maxEccentricity)
                                {
                                    maxEccentricity = eccentricity;
                                    pairs.Clear();
                                }
                                
                                pairs.Enqueue((i, nodes));
                            }
                        }
                        
                    }
                });

            return (maxEccentricity, pairs.SelectMany(p => p.rightNodes.Select(n => (p.leftNode, n))));
        }
    }
}