using UnityEngine;

public class PathGraphInput
{
    private const float DragThreshold = 5f;

    public void Handle(PathGraphContext pathGraphContext, Event clickEvent)
    {
        // LEFT CLICK > SELECT
        if (clickEvent.type == EventType.MouseDown && clickEvent.button == 0)
        {
            pathGraphContext.SelectedNode = pathGraphContext.GetNodeAtPosition(clickEvent.mousePosition);
            clickEvent.Use();
        }

        // RIGHT CLICK START
        if (clickEvent.type == EventType.MouseDown && clickEvent.button == 1)
        {
            pathGraphContext.MouseDownPos = clickEvent.mousePosition;
            pathGraphContext.IsPanning = false;
        }

        // RIGHT CLICK DRAG > PAN
        if (clickEvent.type == EventType.MouseDrag && clickEvent.button == 1)
        {
            if ((clickEvent.mousePosition - pathGraphContext.MouseDownPos).magnitude > DragThreshold)
            {
                pathGraphContext.IsPanning = true;
                pathGraphContext.Offset += clickEvent.delta;
                clickEvent.Use();
            }
        }

        // RIGHT CLICK RELEASE → CONTEXT MENU
        if (clickEvent.type == EventType.MouseUp && clickEvent.button == 1)
        {
            if (!pathGraphContext.IsPanning)
            {
                var node = pathGraphContext.GetNodeAtPosition(clickEvent.mousePosition);
                if (node)
                {
                    pathGraphContext.ShowContextMenu(node);
                }
            }

            pathGraphContext.IsPanning = false;
        }
    }
}