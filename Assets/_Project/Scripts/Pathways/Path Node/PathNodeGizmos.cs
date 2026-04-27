using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNodeGizmos : MonoBehaviour
{
    [SerializeField] private PathNode node;
    [SerializeField] private Color incomingColor = Color.cyan;
    [SerializeField] private Color outgoingColor = Color.green;
    [SerializeField] private float size = 0.25f;

    private void OnDrawGizmos()
    {
        if (!node) node = GetComponent<PathNode>();
        if (!node) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(node.Position, size);

        Draw(node.outgoing, outgoingColor);
        DrawReverse(node.incoming, incomingColor);
    }

    private void Draw(List<PathNode> list, Color color)
    {
        Gizmos.color = color;
        foreach (var pathNode in list.Where(pathNode => pathNode))
        {
            Gizmos.DrawLine(node.Position, pathNode.Position);
        }
    }

    private void DrawReverse(List<PathNode> list, Color color)
    {
        Gizmos.color = color;
        foreach (var pathNode in list.Where(pathNode => pathNode))
        {
            Gizmos.DrawLine(pathNode.Position, node.Position);
        }
    }
}
