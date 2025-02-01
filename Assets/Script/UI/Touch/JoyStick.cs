using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image borderImage;
    [SerializeField] private OnScreenStick onScreenStick;

    private CancellationTokenSource cts;

    public void Initialize()
    {
        canvasGroup.DOFade(0, 0.1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        onScreenStick.OnPointerDown(eventData);

        var pos = new Vector3(eventData.position.x, eventData.position.y, GameManager.Instance.UiCamera.focusDistance);
        var mousePosition = GameManager.Instance.UiCamera.ScreenToWorldPoint(pos);
        borderImage.rectTransform.localPosition = transform.InverseTransformPoint(mousePosition);

        canvasGroup.DOFade(1, 0.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onScreenStick.OnDrag(eventData);
    }

    public async void OnPointerUp(PointerEventData eventData)
    {
        cts = new();

        onScreenStick.OnPointerUp(eventData);

        try
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: cts.Token);
            canvasGroup.DOFade(0, 0.1f);
        }
        catch (Exception) { }
    }
}