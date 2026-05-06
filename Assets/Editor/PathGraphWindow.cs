using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PathGraphWindow : EditorWindow
{
    private readonly List<PathNode> _nodes = new();

    private PathGraphContext _context;
    private PathGraphInput _input = new();

    private const float DragThreshold = 5f;

    private enum ViewDirection { Top, Bottom, Left, Right }
    private ViewDirection _view = ViewDirection.Top;

    [MenuItem("Tools/Path Graph")]
    public static void Open()
    {
        GetWindow<PathGraphWindow>("Path Graph");
    }

    private void OnEnable()
    {
        _context = new PathGraphContext
        {
            GetNodeAtPosition = GetNodeAtPosition,
            ShowContextMenu = ShowContextMenu
        };

        RefreshNodes();
    }

    private void RefreshNodes()
    {
        _nodes.Clear();
        _nodes.AddRange(FindObjectsByType<PathNode>(FindObjectsSortMode.None));
    }

    private void OnGUI()
    {
        HandleZoom(Event.current);

        DrawToolbar();
        DrawGrid(20, 0.15f, Color.gray);
        DrawGrid(100, 0.25f, Color.gray);

        HandleInput(Event.current);

        DrawConnections();
        DrawNodes();

        DrawNodeCounter();

        if (GUI.changed) Repaint();
    }

    #region INPUT WRAPPER

    private void HandleInput(Event e)
    {
        _input.Handle(_context, e);
    }

    #endregion

    #region TOOLBAR

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) RefreshNodes();

        if (GUILayout.Button("Center", EditorStyles.toolbarButton))
        {
            _context.Offset = Vector2.zero;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Top", EditorStyles.toolbarButton))
        {
            _view = ViewDirection.Top;
        }

        if (GUILayout.Button("Bottom", EditorStyles.toolbarButton))
        {
            _view = ViewDirection.Bottom;
        }

        if (GUILayout.Button("Left", EditorStyles.toolbarButton))
        {
            _view = ViewDirection.Left;
        }

        if (GUILayout.Button("Right", EditorStyles.toolbarButton))
        {
            _view = ViewDirection.Right;
        }

        GUILayout.EndHorizontal();
    }

    #endregion

    #region ZOOM

    private void HandleZoom(Event handleZoomEvent)
    {
        if (handleZoomEvent.type != EventType.ScrollWheel) return;

        float delta = -handleZoomEvent.delta.y * 0.05f;
        float oldZoom = _context.Zoom;

        _context.Zoom = Mathf.Clamp(_context.Zoom + delta, 0.5f, 2.5f);

        Vector2 mouse = handleZoomEvent.mousePosition;

        Vector2 before = (mouse - _context.Offset) / oldZoom;
        Vector2 after = (mouse - _context.Offset) / _context.Zoom;

        _context.Offset += (after - before) * _context.Zoom;

        handleZoomEvent.Use();
    }

    #endregion

    #region DRAW NODES

    private void DrawNodes()
    {
        var style = PathGraphUtils.GetNodeStyle();

        foreach (var node in _nodes)
        {
            if (!node) continue;

            Vector2 pos = WorldToGUI(node.transform.position);
            Rect rect = PathGraphUtils.GetNodeRect(pos, node.name);

            GUI.Box(rect, GetLabel(node), style);

            if (node == _context.SelectedNode)
            {
                Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.yellow);
            }
        }
    }

    private string GetLabel(PathNode node)
    {
        var vector3 = node.transform.position;
        return $"{node.name}\n({vector3.x:F1}, {vector3.y:F1}, {vector3.z:F1})";
    }

    #endregion

    #region CONNECTIONS

    private void DrawConnections()
    {
        Handles.BeginGUI();

        HashSet<(PathNode, PathNode)> drawn = new();
        float laneGap = 6f;

        foreach (var node in _nodes)
        {
            if (!node) continue;

            foreach (var target in node.outgoing)
            {
                if (!target) continue;

                var nodeTarget = (node, target);
                if (!drawn.Add(nodeTarget)) continue;

                Vector2 from = WorldToGUI(node.transform.position);
                Vector2 to = WorldToGUI(target.transform.position);

                bool hasReverse = target.outgoing.Contains(node);

                if (hasReverse)
                {
                    DrawArrow(from, to, Color.green, -laneGap);
                    DrawArrow(to, from, Color.red, -laneGap);

                    drawn.Add((target, node));
                }
                else
                {
                    // One-way outgoing = green, centered
                    DrawArrow(from, to, Color.green, 0f);
                }
            }
        }

        Handles.EndGUI();

    }
    
    private void DrawArrow(Vector2 from, Vector2 to, Color color, float sideOffset)
    {
        Vector2 dir = (to - from).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        float nodePadding = 30f;

        Vector2 start = from + dir * nodePadding + perp * sideOffset;
        Vector2 end = to - dir * nodePadding + perp * sideOffset;

        Handles.color = color;
        Handles.DrawLine(start, end);

        DrawArrowHead(start, end, color);
    }

    private void DrawArrowHead(Vector2 start, Vector2 end, Color color)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 right = new Vector2(dir.y, -dir.x);

        float size = 10f;

        Vector2 p1 = end;
        Vector2 p2 = end - dir * size + right * (size * 0.5f);
        Vector2 p3 = end - dir * size - right * (size * 0.5f);

        Handles.color = color;
        Handles.DrawAAConvexPolygon(p1, p2, p3);
    }

    #endregion

    #region VIEW (FLATTENED)

    Vector2 WorldToGUI(Vector3 world)
    {
        Vector2 flat;

        switch (_view)
        {
            case ViewDirection.Top:
                flat = new Vector2(world.x, -world.z);
                break;
            case ViewDirection.Bottom:
                flat = new Vector2(world.x, world.z);
                break;
            case ViewDirection.Left:
                flat = new Vector2(world.z, 0);
                break;
            case ViewDirection.Right:
                flat = new Vector2(-world.z, 0);
                break;
            default:
                flat = new Vector2(world.x, -world.z);
                break;
        }

        return (flat * (50f * _context.Zoom))
               + _context.Offset
               + new Vector2(position.width * 0.5f, position.height * 0.5f);
    }

    #endregion

    #region NODE PICKING

    PathNode GetNodeAtPosition(Vector2 mousePos)
    {
        foreach (var node in _nodes)
        {
            Vector2 pos = WorldToGUI(node.transform.position);
            Rect rect = PathGraphUtils.GetNodeRect(pos, node.name);

            if (rect.Contains(mousePos))
            {
                return node;
            }
        }

        return null;
    }

    #endregion

    #region CONTEXT MENU

    private void ShowContextMenu(PathNode target)
    {
        GenericMenu menu = new GenericMenu();

        if (_context.SelectedNode && _context.SelectedNode != target)
        {
            menu.AddItem(new GUIContent("Connect Selected → This"), false, () =>
            {
                Connect(_context.SelectedNode, target);
            });
        }

        if (_context.SelectedNode && _context.SelectedNode != target)
        {
            if (_context.SelectedNode.outgoing.Contains(target))
            {
                menu.AddItem(new GUIContent("Disconnect Selected → This"), false, () =>
                {
                    Disconnect(_context.SelectedNode, target);
                });
            }
        }

        menu.ShowAsContext();
    }

    private void Connect(PathNode pathNodeA, PathNode pathNodeB)
    {
        Undo.RecordObject(pathNodeA, "Connect");
        Undo.RecordObject(pathNodeB, "Connect");

        if (!pathNodeA.outgoing.Contains(pathNodeB))
        {
            pathNodeA.outgoing.Add(pathNodeB);
        }

        if (!pathNodeB.incoming.Contains(pathNodeA))
        {
            pathNodeB.incoming.Add(pathNodeA);
        }

        EditorUtility.SetDirty(pathNodeA);
        EditorUtility.SetDirty(pathNodeB);
    }
    
    private void Disconnect(PathNode pathNodeA, PathNode pathNodeB)
    {
        Undo.RecordObject(pathNodeA, "Disconnect Nodes");
        Undo.RecordObject(pathNodeB, "Disconnect Nodes");

        if (pathNodeA.outgoing.Contains(pathNodeB))
        {
            pathNodeA.outgoing.Remove(pathNodeB);
        }

        if (pathNodeB.incoming.Contains(pathNodeA))
        {
            pathNodeB.incoming.Remove(pathNodeA);
        }

        EditorUtility.SetDirty(pathNodeA);
        EditorUtility.SetDirty(pathNodeB);
    }

    #endregion

    #region GRID

    private void DrawGrid(float spacing, float opacity, Color color)
    {
        Handles.BeginGUI();

        Handles.color = new Color(color.r, color.g, color.b, opacity);

        Vector3 offsetGrid = new Vector3(_context.Offset.x % spacing, _context.Offset.y % spacing);

        int width = Mathf.CeilToInt(position.width / spacing);
        int height = Mathf.CeilToInt(position.height / spacing);

        for (int i = 0; i < width; i++)
        {
            Handles.DrawLine(new Vector3(spacing * i, 0) + offsetGrid,
                new Vector3(spacing * i, position.height) + offsetGrid);
        }

        for (int j = 0; j < height; j++)
        {
            Handles.DrawLine(new Vector3(0, spacing * j) + offsetGrid, 
                new Vector3(position.width, spacing * j) + offsetGrid);
        }

        Handles.EndGUI();
    }

    #endregion

    #region UI COUNTER

    private void DrawNodeCounter()
    {
        Rect rect = new Rect(10, position.height - 40, 140, 30);
        GUI.Box(rect, $"Nodes: {_nodes.Count}");
    }

    #endregion
}