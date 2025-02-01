using UnityEngine;

public class ScrollRectVerticalIndicator : ScrollRectScrollIndicator
{
    [SerializeField] private GameObject upScrollIndicator;
    [SerializeField] private GameObject downScrollIndicator;

    private void LateUpdate()
    {
        if (scrollRect == null)
            return;

        if (vScrollingNeeded == false)
        {
            if (downScrollIndicator != null)
                downScrollIndicator.gameObject.DeActive();

            if (upScrollIndicator != null)
                upScrollIndicator.gameObject.DeActive();

            return;
        }

        if (scrollRect.normalizedPosition.y <= 0.1f)
        {
            if (downScrollIndicator != null)
                downScrollIndicator.gameObject.DeActive();

            if (upScrollIndicator != null)
                upScrollIndicator.gameObject.Active();
        }
        else
        {
            if (downScrollIndicator != null)
                downScrollIndicator.gameObject.Active();

            if (upScrollIndicator != null)
                upScrollIndicator.gameObject.DeActive();
        }
    }
}