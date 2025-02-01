using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectDragDirectionDetector : MonoBehaviour, IDragHandler
{
    [SerializeField] private ScrollRect scrollRect;

    private Vector2 previousOnDragPosition;
    private Vector2 normalizedPosition => scrollRect.normalizedPosition;
    public ScrollDirectionEnum ScrollDirectionEnum { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollRect == null)
            return;

        // Get the current normalized position
        Vector2 currentNormalizedPosition = normalizedPosition;

        // Check if the normalized position is increasing or decreasing
        if (currentNormalizedPosition.x > previousOnDragPosition.x)
        {
            // Normalized position is increasing towards 1
            ScrollDirectionEnum = ScrollDirectionEnum.Right;
        }
        else if (currentNormalizedPosition.x < previousOnDragPosition.x)
        {
            // Normalized position is decreasing towards 0
            ScrollDirectionEnum = ScrollDirectionEnum.Left;
        }

        // Check if the normalized position is increasing or decreasing
        if (currentNormalizedPosition.y > previousOnDragPosition.y)
        {
            // Normalized position is increasing towards 1
            ScrollDirectionEnum = ScrollDirectionEnum.Down;
        }
        else if (currentNormalizedPosition.y < previousOnDragPosition.y)
        {
            // Normalized position is decreasing towards 0
            ScrollDirectionEnum = ScrollDirectionEnum.Up;
        }

        // Update the previous normalized position
        previousOnDragPosition = currentNormalizedPosition;
    }
}