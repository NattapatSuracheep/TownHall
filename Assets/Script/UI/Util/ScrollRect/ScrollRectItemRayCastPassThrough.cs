using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectItemRayCastPassThrough : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;

    [Header("Optional")]
    [Tooltip("temporary disable interactable when dragging")]
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetScrollRect(ScrollRect scrollRect)
    {
        this.scrollRect = scrollRect;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        scrollRect?.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect?.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect?.OnEndDrag(eventData);

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}