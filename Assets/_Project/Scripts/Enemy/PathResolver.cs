using System.Collections.Generic;
using UnityEngine;

public static class PathResolver
{
    public static List<PathNode> FindPath(PathNode start, PathNode goal)
    {
        var open = new List<PathNode> { start };
        var cameFrom = new Dictionary<PathNode, PathNode>();
        var cost = new Dictionary<PathNode, float> { [start] = 0 };

        while (open.Count > 0)
        {
            var current = GetLowest(open, cost);

            if (current == goal)
            {
                return Reconstruct(cameFrom, current);
            }

            open.Remove(current);

            foreach (var next in current.outgoing)
            {
                if (!next) continue;

                float newCost = cost[current] + Vector3.Distance(current.Position, next.Position);

                if (cost.ContainsKey(next) && cost[next] <= newCost) continue;

                cost[next] = newCost;
                cameFrom[next] = current;

                if (!open.Contains(next))
                {
                    open.Add(next);
                }
            }
        }

        return null;
    }

    static PathNode GetLowest(List<PathNode> list, Dictionary<PathNode, float> cost)
    {
        PathNode best = null;
        float bestCost = float.MaxValue;

        foreach (var pathNode in list)
        {
            if (!(cost[pathNode] < bestCost)) continue;
            best = pathNode;
            bestCost = cost[pathNode];
        }

        return best;
    }

    static List<PathNode> Reconstruct(Dictionary<PathNode, PathNode> came, PathNode current)
    {
        var path = new List<PathNode>();

        while (came.ContainsKey(current))
        {
            path.Add(current);
            current = came[current];
        }

        path.Add(current);
        path.Reverse();
        return path;
    }
}
