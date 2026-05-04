using System.Collections.Generic;
using UnityEngine;

public static class PathResolver
{
    public static List<PathNode> FindPath(PathNode start, PathNode target, PathPreferenceTypeEnum preference)
    {
        if (!start || !target)
        {
            return null;
        }

        var openSet = new List<PathNode> { start };
        var cameFrom = new Dictionary<PathNode, PathNode>();

        var gScore = new Dictionary<PathNode, float>();
        var fScore = new Dictionary<PathNode, float>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);

        while (openSet.Count > 0)
        {
            PathNode current = GetLowestFScore(openSet, fScore);

            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (var neighbor in current.outgoing)
            {
                float cost = Distance(current, neighbor);

                // NEW: node cost system
                if (neighbor.costData)
                {
                    cost += neighbor.costData.GetTotalCost();
                }

                // Preference influence
                cost += GetPreferenceCost(neighbor, preference);

                float tentativeG = gScore[current] + cost;

                if (gScore.ContainsKey(neighbor) && !(tentativeG < gScore[neighbor])) continue;
                
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, target);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }
        }

        return null;
    }
    
    private static float Heuristic(PathNode a, PathNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static float Distance(PathNode a, PathNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static PathNode GetLowestFScore(List<PathNode> nodes, Dictionary<PathNode, float> fScore)
    {
        PathNode best = nodes[0];
        float bestScore = fScore.ContainsKey(best) ? fScore[best] : float.MaxValue;

        foreach (var pathNode in nodes)
        {
            float score = fScore.ContainsKey(pathNode) ? fScore[pathNode] : float.MaxValue;

            if (!(score < bestScore)) continue;
            
            best = pathNode;
            bestScore = score;
        }

        return best;
    }

    private static List<PathNode> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode current)
    {
        List<PathNode> path = new();

        while (current)
        {
            path.Add(current);
            cameFrom.TryGetValue(current, out current);
        }

        path.Reverse();
        return path;
    }
    
    private static float GetPreferenceCost(PathNode node, PathPreferenceTypeEnum preference)
    {
        float cost = 0f;

        switch (preference)
        {
            case PathPreferenceTypeEnum.Random:
                cost += Random.Range(0f, 1.5f);
                break;

            case PathPreferenceTypeEnum.Shortest:
                // pure A*
                break;

            case PathPreferenceTypeEnum.Safest:
                if (node.attachedBase)
                {
                    cost += 8f;
                }
                break;

            case PathPreferenceTypeEnum.Aggressive:
                if (node.attachedBase)
                {
                    cost -= 4f;
                }
                break;
        }

        return cost;
    }
}
