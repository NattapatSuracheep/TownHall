using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectDropdownSizeFix : MonoBehaviour
{
    [SerializeField] private float minHeight = 90f;
    [SerializeField] private float maxHeight = 300f;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ScrollRect scrollRect;

    private void LateUpdate()
    {
        Calculate();
    }

    private void Calculate()
    {
        // Calculate the preferred size
        Vector2 preferredSize = new Vector2(scrollRect.content.rect.width, scrollRect.content.rect.height);

        // Apply the maximum height
        preferredSize.y = Mathf.Clamp(preferredSize.y, minHeight, maxHeight);

        // Set the size
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
    }
}
