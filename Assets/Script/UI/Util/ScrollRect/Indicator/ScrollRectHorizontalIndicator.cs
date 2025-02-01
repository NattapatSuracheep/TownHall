using UnityEngine;

public class ScrollRectHorizontalIndicator : ScrollRectScrollIndicator
{
    [SerializeField] private GameObject leftScrollIndicator;
    [SerializeField] private GameObject rightScrollIndicator;

    private void LateUpdate()
    {
        if (scrollRect == null)
            return;

        if (hScrollingNeeded == false)
        {
            if (leftScrollIndicator != null)
                leftScrollIndicator.gameObject.DeActive();

            if (rightScrollIndicator != null)
                rightScrollIndicator.gameObject.DeActive();

            return;
        }

        if (scrollRect.normalizedPosition.x <= 0.1f)
        {
            if (leftScrollIndicator != null)
                leftScrollIndicator.DeActive();

            if (rightScrollIndicator != null)
                rightScrollIndicator.Active();
        }
        else if (scrollRect.normalizedPosition.x >= 0.9f)
        {
            if (leftScrollIndicator != null)
                leftScrollIndicator.Active();

            if (rightScrollIndicator != null)
                rightScrollIndicator?.DeActive();
        }
        else
        {
            if (leftScrollIndicator != null)
                leftScrollIndicator?.Active();

            if (rightScrollIndicator != null)
                rightScrollIndicator?.Active();
        }
    }
}