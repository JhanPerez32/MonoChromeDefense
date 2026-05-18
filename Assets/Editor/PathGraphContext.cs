using UnityEngine;
using System;

public class PathGraphContext
{
    public PathNode SelectedNode;

    public Vector2 Offset;
    public float Zoom = 1f;

    public bool IsPanning;
    public Vector2 MouseDownPos;

    public Func<Vector2, PathNode> GetNodeAtPosition;
    public Action<PathNode> ShowContextMenu;
}