using UnityEngine;

public static class PathGraphUtils
{
    public static Rect GetNodeRect(Vector2 pos, string name)
    {
        float width = Mathf.Clamp(name.Length * 7f, 120f, 320f);
        float height = 55f;

        return new Rect(
            pos.x - width * 0.5f,
            pos.y - height * 0.5f,
            width,
            height
        );
    }

    public static GUIStyle GetNodeStyle()
    {
        return new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = { textColor = Color.white }
        };
    }
}
