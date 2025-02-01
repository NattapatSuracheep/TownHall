using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public abstract class ScrollRectScrollIndicator : MonoBehaviour
{
    [SerializeField] protected ScrollRect scrollRect;

    protected bool hScrollingNeeded
    {
        get
        {
            if (Application.isPlaying)
            {
                return scrollRect.content.rect.size.x > scrollRect.viewport.rect.size.x + 0.01f;
            }

            return true;
        }
    }

    protected bool vScrollingNeeded
    {
        get
        {
            if (Application.isPlaying)
            {
                return scrollRect.content.rect.size.y > scrollRect.viewport.rect.size.y + 0.01f;
            }

            return true;
        }
    }
}