using System.Collections.Generic;
using UnityEngine;

public static class PathBlockingUtility
{
    public static PlayerBase GetBlockingBase(PathNode startNode, PlayerBase intendedTarget)
    {
        if (startNode == null) return null;

        HashSet<PathNode> visited = new();
        Queue<PathNode> queue = new();

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            PathNode node = queue.Dequeue();

            if (!visited.Add(node)) continue;

            // If a base is found BEFORE the intended target > block
            if (node.attachedBase != null && node.attachedBase != intendedTarget)
            {
                return node.attachedBase;
            }

            foreach (var next in node.outgoing)
            {
                if (next != null)
                {
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }
}
