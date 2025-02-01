using UnityEngine;

public static class CanvasGroupExtension
{
    public static void Show(this CanvasGroup group)
    {
        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public static void Hide(this CanvasGroup group)
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
}