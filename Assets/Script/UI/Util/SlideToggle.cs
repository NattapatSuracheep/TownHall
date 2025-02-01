using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SlideToggle : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float duration = 1f;

    [SerializeField] private Toggle toggle;

    [Header("Graphics")]
    [SerializeField] private GameObject onImage;
    [SerializeField] private GameObject offImage;
    [SerializeField] private RectTransform handleImage;

    [Header("Bouncing")]
    [SerializeField] private float handleSizeIncreasePercent = 0.6f;
    [SerializeField] private float handleSizeIncreaseDuration = 0.1f;
    [SerializeField] private float handleChangeSideDuration = 0.6f;

    private Vector2 originHandleScale;

    public event Action<bool> onValueChanged;

    private void Start()
    {
        Initialize(false);
    }

    public void Initialize(bool startValue)
    {
        originHandleScale = handleImage.localScale;

        toggle.isOn = startValue;
        HandleGraphic(startValue);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        onValueChanged?.Invoke(value);
        HandleGraphic(value);
    }

    private void HandleGraphic(bool value)
    {
        if (value)
        {
            if (handleImage.pivot.x != 1)
            {
                // handleImage.pivot = new Vector2(0, handleImage.pivot.y);
                handleImage.anchorMin = new Vector2(1, 0);
                handleImage.anchorMax = new Vector2(1, 1);
                handleImage.anchoredPosition = new Vector2(-(handleImage.localPosition.x * 2), handleImage.localPosition.y);
                // handleImage.localPosition = new Vector2(-handleImage.rect.width, handleImage.anchoredPosition.y);
            }

            HandleTransitionAnimation(0, 1, new Vector2(originHandleScale.x + (originHandleScale.x * handleSizeIncreasePercent), originHandleScale.y));
        }
        else
        {
            if (handleImage.pivot.x != 0)
            {
                // handleImage.pivot = new Vector2(1, handleImage.pivot.y);
                handleImage.anchorMin = new Vector2(0, 0);
                handleImage.anchorMax = new Vector2(0, 1);
                handleImage.anchoredPosition = new Vector2(-(handleImage.localPosition.x * 2), handleImage.localPosition.y);
                // handleImage.localPosition = new Vector2(handleImage.rect.width, handleImage.anchoredPosition.y);
            }

            HandleTransitionAnimation(0, 0, new Vector2(originHandleScale.x + (originHandleScale.x * handleSizeIncreasePercent), originHandleScale.y));
        }

    }

    private void HandleTransitionAnimation(float posX, float pivot, Vector2 sizeDelta)
    {
        toggle.interactable = false;

        var handleSequence = DOTween.Sequence();
        handleSequence.Append(handleImage.DOScale(sizeDelta, duration * handleSizeIncreaseDuration));
        handleSequence.Append(handleImage.DOScale(originHandleScale, duration * (1f - handleSizeIncreaseDuration)));
        handleSequence.SetEase(Ease.InOutExpo);

        var sequence = DOTween.Sequence();
        sequence.Append(handleImage.DOAnchorPosX(posX, duration * handleChangeSideDuration));
        sequence.Join(handleImage.DOPivot(new Vector2(pivot, handleImage.pivot.y), duration * (1f - handleChangeSideDuration)));
        sequence.AppendCallback(() =>
        {
            toggle.interactable = true;
        });
        sequence.SetEase(Ease.InOutExpo);

        var imageSequence = DOTween.Sequence();
        imageSequence.AppendInterval(duration * 0.5f);
        imageSequence.AppendCallback(() =>
        {
            if (toggle.isOn)
            {
                onImage.Active();
                offImage.DeActive();
            }
            else
            {
                onImage.DeActive();
                offImage.Active();
            }
        });

        handleSequence.Play();
        sequence.Play();
        imageSequence.Play();
    }

    private Vector2 previousOnDragPosition;
    public ScrollDirectionEnum ScrollDirectionEnum { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        // Get the current normalized position
        Vector2 currentNormalizedPosition = eventData.position;

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

        // Update the previous normalized position
        previousOnDragPosition = currentNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!toggle.interactable)
            return;

        if (ScrollDirectionEnum == ScrollDirectionEnum.Right)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }
}