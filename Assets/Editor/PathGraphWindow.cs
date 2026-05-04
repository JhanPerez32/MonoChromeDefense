using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PathGraphWindow : EditorWindow
{
    private readonly List<PathNode> _nodes = new();

    private PathNode _selectedNode;

    private Vector2 _offset;
    private float _zoom = 1f;

    private bool _isDragging;
    private Vector2 _mouseDownPos;

    private const float DragThreshold = 5f;
    
    private enum ViewDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private ViewDirection _view = ViewDirection.Top;

    [MenuItem("Tools/Path Graph")]
    public static void Open()
    {
        GetWindow<PathGraphWindow>("Path Graph");
    }

    private void OnEnable()
    {
        RefreshNodes();
    }

    private void RefreshNodes()
    {
        _nodes.Clear();
        _nodes.AddRange(FindObjectsOfType<PathNode>());
    }

    private void OnGUI()
    {
        HandleZoom(Event.current);

        DrawToolbar();

        DrawGrid(20, 0.15f, Color.gray);
        DrawGrid(100, 0.25f, Color.gray);

        DrawOrientationGuide();

        DrawConnections();
        DrawNodes();

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    #region TOOLBAR

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            RefreshNodes();
        }

        if (GUILayout.Button("Deselect", EditorStyles.toolbarButton))
        {
            _selectedNode = null;
        }

        if (GUILayout.Button("Center", EditorStyles.toolbarButton))
        {
            CenterView();
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

    private void CenterView()
    {
        _offset = Vector2.zero;
        _zoom = 1f;
    }

    #endregion

    #region ZOOM

    private void HandleZoom(Event handleZoomEvent)
    {
        if (handleZoomEvent.type != EventType.ScrollWheel) return;
        
        float delta = -handleZoomEvent.delta.y * 0.05f;
        float oldZoom = _zoom;

        _zoom = Mathf.Clamp(_zoom + delta, 0.5f, 2.5f);

        Vector2 mouse = handleZoomEvent.mousePosition;

        Vector2 before = (mouse - _offset) / oldZoom;
        Vector2 after = (mouse - _offset) / _zoom;

        _offset += (after - before) * _zoom;

        handleZoomEvent.Use();
    }

    #endregion

    #region INPUT (FIXED DRAG + SELECTION)

    private void ProcessEvents(Event processEvent)
    {
        if (processEvent.type == EventType.MouseDown && processEvent.button == 0)
        {
            _mouseDownPos = processEvent.mousePosition;
            _isDragging = false;

            _selectedNode = GetNodeAtPosition(processEvent.mousePosition);

            processEvent.Use();
        }

        if (processEvent.type == EventType.MouseDrag && processEvent.button == 0)
        {
            if ((processEvent.mousePosition - _mouseDownPos).magnitude > DragThreshold)
            {
                _isDragging = true;
                _offset += processEvent.delta;
                processEvent.Use();
            }
        }

        if (processEvent.type == EventType.MouseUp && processEvent.button == 0)
        {
            _isDragging = false;
        }

        if (processEvent.type != EventType.MouseDown || processEvent.button != 1) return;
        PathNode target = GetNodeAtPosition(processEvent.mousePosition);

        if (target)
        {
            ShowContextMenu(target);
        }

        processEvent.Use();
    }

    #endregion

    #region DRAW NODES

    private void DrawNodes()
    {
        foreach (var node in _nodes)
        {
            if (!node) continue;

            Vector2 pos = WorldToGUI(node.transform.position);

            float width = Mathf.Clamp(node.name.Length * 7f, 120f, 320f);
            float height = 55f;

            Rect rect = new Rect(
                pos.x - width * 0.5f,
                pos.y - height * 0.5f,
                width,
                height
            );

            GUI.color = (node == _selectedNode) ? Color.yellow : Color.white;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            string label =
                $"{node.name}\n" +
                $"({node.transform.position.x:F1}, {node.transform.position.y:F1}, {node.transform.position.z:F1})";

            GUI.Box(rect, label, style);

            GUI.color = Color.white;
        }
    }

    #endregion

    #region CONNECTIONS

    private void DrawConnections()
    {
        Handles.BeginGUI();

        foreach (var node in _nodes)
        {
            if (!node) continue;

            Vector2 from = WorldToGUI(node.transform.position);

            foreach (var target in node.outgoing)
            {
                if (!target) continue;

                DrawArrow(from, WorldToGUI(target.transform.position), Color.red);
            }

            foreach (var incoming in node.incoming)
            {
                if (!incoming) continue;

                DrawArrow(WorldToGUI(incoming.transform.position), from, Color.green);
            }
        }

        Handles.EndGUI();
    }

    private void DrawArrow(Vector2 from, Vector2 to, Color color)
    {
        Vector2 dir = (to - from).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        Vector2 start = from + perp * 6f;
        Vector2 end = to + perp * 6f;

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

    #region VIEW (TRUE SCENE-LIKE PROJECTION)

    Vector2 WorldToGUI(Vector3 world)
    {
        Vector3 vector3;

        switch (_view)
        {
            case ViewDirection.Top:
                // Scene View Top = looking from +Y
                vector3 = new Vector3(world.x, -world.z, 0);
                break;

            case ViewDirection.Bottom:
                vector3 = new Vector3(world.x, world.z, 0);
                break;

            case ViewDirection.Left:
                vector3 = new Vector3(world.z, -world.x, 0);
                break;

            case ViewDirection.Right:
                vector3 = new Vector3(-world.z, -world.x, 0);
                break;

            default:
                vector3 = new Vector3(world.x, -world.z, 0);
                break;
        }

        return (new Vector2(vector3.x, vector3.y) * (50f * _zoom))
               + _offset
               + new Vector2(position.width * 0.5f, position.height * 0.5f);
    }

    #endregion

    #region ORIENTATION GUIDE

    private void DrawOrientationGuide()
    {
        Handles.BeginGUI();

        Vector2 c = new Vector2(100, 80);

        Handles.color = Color.green;
        Handles.DrawLine(c, c + Vector2.up * 30);
        Handles.Label(c + Vector2.up * 40, "TOP / +Z");

        Handles.color = Color.red;
        Handles.DrawLine(c, c + Vector2.down * 30);
        Handles.Label(c + Vector2.down * 40, "BOTTOM / -Z");

        Handles.color = Color.blue;
        Handles.DrawLine(c, c + Vector2.right * 30);
        Handles.Label(c + Vector2.right * 40, "RIGHT / +X");

        Handles.color = Color.yellow;
        Handles.DrawLine(c, c + Vector2.left * 30);
        Handles.Label(c + Vector2.left * 40, "LEFT / -X");

        Handles.EndGUI();
    }

    #endregion

    #region HELPERS

    PathNode GetNodeAtPosition(Vector2 mousePos)
    {
        foreach (var node in _nodes)
        {
            Vector2 pos = WorldToGUI(node.transform.position);

            float width = Mathf.Clamp(node.name.Length * 7f, 120f, 320f);

            Rect rect = new Rect(pos.x - width * 0.5f, pos.y - 25f, width, 55f);

            if (rect.Contains(mousePos))
                return node;
        }

        return null;
    }

    #endregion

    #region GRID

    private void DrawGrid(float spacing, float opacity, Color color)
    {
        Handles.BeginGUI();

        Handles.color = new Color(color.r, color.g, color.b, opacity);

        Vector3 offsetGrid = new Vector3(_offset.x % spacing, _offset.y % spacing, 0);

        int w = Mathf.CeilToInt(position.width / spacing);
        int h = Mathf.CeilToInt(position.height / spacing);

        for (int i = 0; i < w; i++)
        {
            Handles.DrawLine(
                new Vector3(spacing * i, 0, 0) + offsetGrid,
                new Vector3(spacing * i, position.height, 0) + offsetGrid
            );
        }

        for (int j = 0; j < h; j++)
        {
            Handles.DrawLine(
                new Vector3(0, spacing * j, 0) + offsetGrid,
                new Vector3(position.width, spacing * j, 0) + offsetGrid
            );
        }

        Handles.EndGUI();
    }

    #endregion

    #region CONNECTION LOGIC

    private void ShowContextMenu(PathNode target)
    {
        GenericMenu menu = new GenericMenu();

        if (_selectedNode && _selectedNode != target)
        {
            menu.AddItem(new GUIContent("Connect Selected → This"), false, () =>
            {
                Connect(_selectedNode, target);
            });
        }

        if (_selectedNode && _selectedNode.outgoing.Contains(target))
        {
            menu.AddItem(new GUIContent("Delete Connection"), false, () =>
            {
                RemoveConnection(_selectedNode, target);
            });
        }

        menu.ShowAsContext();
    }

    private void Connect(PathNode a, PathNode b)
    {
        Undo.RecordObject(a, "Connect Nodes");
        Undo.RecordObject(b, "Connect Nodes");

        if (!a.outgoing.Contains(b))
            a.outgoing.Add(b);

        if (!b.incoming.Contains(a))
            b.incoming.Add(a);

        EditorUtility.SetDirty(a);
        EditorUtility.SetDirty(b);
    }

    private void RemoveConnection(PathNode pathNodeA, PathNode pathNodeB)
    {
        Undo.RecordObject(pathNodeA, "Remove Connection");
        Undo.RecordObject(pathNodeB, "Remove Connection");

        pathNodeA.outgoing.Remove(pathNodeB);
        pathNodeB.incoming.Remove(pathNodeA);

        EditorUtility.SetDirty(pathNodeA);
        EditorUtility.SetDirty(pathNodeB);
    }

    #endregion
}