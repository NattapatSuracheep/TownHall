using UnityEditor;
using UnityEngine;

public class EditorWindowsHelper
{
    public static void HorizontalLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    public static void BreakLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        rect.center = rect.position + new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 0);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
    }
}