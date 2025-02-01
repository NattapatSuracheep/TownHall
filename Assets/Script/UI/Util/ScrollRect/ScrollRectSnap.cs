using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GamesTan.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollRectSnap : SuperScrollRect
{
    private const float lerpTime = 0.2f;

    [SerializeField] private float snapThresholdPercent = 0.1f;
    [SerializeField] private PageIndicatorController pageIndicatorController;
    [SerializeField] private ScrollRectDragDirectionDetector scrollRectDragDirection;

    private Sequence lerpSequence;

    private List<RectTransform> itemList = new();
    private Dictionary<int, float> itemSnapPositionDict = new();
    private Dictionary<int, float> itemScrollPositionNormalizedDict = new();
    private int snapIndex = -1;
    private float itemNormalizePositionBetweenEachItem;
    private float originDecelerationRate;
    private bool isScrollInvert;
    private float snapRightThresholdPercent => itemNormalizePositionBetweenEachItem * (1 - snapThresholdPercent);
    private float snapLeftThresholdPercent => itemNormalizePositionBetweenEachItem * snapThresholdPercent;
    private ScrollDirectionEnum scrollDirection => scrollRectDragDirection.ScrollDirectionEnum;

    public event Action<int> OnSnap;
    public float SnapThresholdPercent => snapThresholdPercent;
    public int CurrentItemIndex { get; private set; }

    protected override void Start()
    {
        base.Start();

        if (!Application.isPlaying)
            return;

        originDecelerationRate = decelerationRate;
    }

    public void ChangeSnapThresholdPercent(float snapThresholdPercent)
    {
        this.snapThresholdPercent = snapThresholdPercent;
    }

    public async UniTask RefreshAsync()
    {
        if (!ValidateContentPivot())
            return;

        StoreContentChild();
        RescaleEachItem();

        await UniTask.NextFrame(); //wait for Unity layout component to re-arrange the item in content

        CalculateEachItemSnapPosition();
        CalculateEachItemNormalizedPosition();

        await pageIndicatorController.InitializeAsync(itemList.Count);
        CheckInvert();
    }

    private void StoreContentChild()
    {
        itemList.Clear();

        for (var i = 0; i < content.childCount; i++)
            itemList.Add(content.GetChild(i).GetComponent<RectTransform>());
    }

    private void RescaleEachItem()
    {
        for (var i = 0; i < itemList.Count; i++)
        {
            var item = itemList[i];
            item.sizeDelta = viewport.rect.size;
        }
    }

    private void CalculateEachItemSnapPosition()
    {
        itemSnapPositionDict.Clear();

        var firstItem = itemList[0];

        if (horizontal)
        {
            var offSetPosition = 0f;
            if (firstItem.pivot.x != 0)
                offSetPosition = itemList[0].anchoredPosition.x;

            for (var i = 0; i < itemList.Count; i++)
                itemSnapPositionDict.Add(i, -(itemList[i].anchoredPosition.x - offSetPosition));
        }
        else if (vertical)
        {
            var offSetPosition = 0f;
            if (firstItem.pivot.y != 1)
                offSetPosition = itemList[0].anchoredPosition.y;

            for (var i = 0; i < itemList.Count; i++)
                itemSnapPositionDict.Add(i, -(itemList[i].anchoredPosition.y - offSetPosition));
        }

        Log.Array(itemSnapPositionDict);
    }

    private void CalculateEachItemNormalizedPosition()
    {
        itemNormalizePositionBetweenEachItem = (float)Math.Round(1f / (itemList.Count - 1), 2);

        itemScrollPositionNormalizedDict.Clear();

        if (horizontal)
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var normalizedPosition = itemNormalizePositionBetweenEachItem * i;
                itemScrollPositionNormalizedDict.Add(i, normalizedPosition);
            }
        }
        else if (vertical)
        {
            for (var i = itemList.Count; i > 0; i--)
            {
                var normalizedPosition = itemNormalizePositionBetweenEachItem * (i - 1);
                itemScrollPositionNormalizedDict.Add(itemList.Count - i, normalizedPosition);
            }
        }

        Log.Array(itemScrollPositionNormalizedDict);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        decelerationRate = originDecelerationRate;
        lerpSequence?.Kill();

        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        decelerationRate = 0;

        base.OnEndDrag(eventData);

        OnScroll(normalizedPosition);
    }

    private void OnScroll(Vector2 scrollValue)
    {
        if (horizontal)
            CalculateHorizontalSnap(scrollValue);
        else if (vertical)
            CalculateVerticalSnap(scrollValue);
    }

    private void CalculateHorizontalSnap(Vector2 scrollValue)
    {
        var scrollValuePosition = scrollValue.x;

        if (scrollDirection == ScrollDirectionEnum.Right)
        {
            // Snap to the right one by one
            if (CurrentItemIndex < itemScrollPositionNormalizedDict.Count - 1)
            {
                var nextItemNormalizedPos = itemScrollPositionNormalizedDict[CurrentItemIndex + 1];
                var normalizedThreshold = nextItemNormalizedPos - snapRightThresholdPercent;

                if (scrollValuePosition > normalizedThreshold)
                {
                    CurrentItemIndex++;
                }
            }
        }
        else if (scrollDirection == ScrollDirectionEnum.Left)
        {
            // Snap to the left one by one
            if (CurrentItemIndex > 0)
            {
                var currentItemNormalizedPos = itemScrollPositionNormalizedDict[CurrentItemIndex];
                var normalizedThreshold = currentItemNormalizedPos - snapLeftThresholdPercent;

                if (scrollValuePosition < normalizedThreshold)
                {
                    CurrentItemIndex--;
                }
            }
        }

        SnapHorizontalTo(CurrentItemIndex);
    }

    private void CalculateVerticalSnap(Vector2 scrollValue)
    {
        Log.Logging(scrollValue);
        Log.Logging(scrollDirection);

        var scrollValuePosition = scrollValue.y;

        if (scrollDirection == ScrollDirectionEnum.Up)
        {
            // Snap to the up one by one
            if (CurrentItemIndex < itemScrollPositionNormalizedDict.Count - 1)
            {
                var nextItemNormalizedPos = itemScrollPositionNormalizedDict[CurrentItemIndex + 1];
                var normalizedThreshold = nextItemNormalizedPos + snapRightThresholdPercent;

                if (scrollValuePosition < normalizedThreshold)
                {
                    CurrentItemIndex++;
                }
            }
        }
        else if (scrollDirection == ScrollDirectionEnum.Down)
        {
            // Snap to the down one by one
            if (CurrentItemIndex > 0)
            {
                var currentItemNormalizedPos = itemScrollPositionNormalizedDict[CurrentItemIndex];
                var normalizedThreshold = currentItemNormalizedPos + snapLeftThresholdPercent;

                if (scrollValuePosition > normalizedThreshold)
                {
                    CurrentItemIndex--;
                }
            }
        }

        SnapVerticalTo(CurrentItemIndex);
    }

    public void SnapHorizontalTo(int index)
    {
        if (index == snapIndex)
            return;

        if (index < 0 || index > itemSnapPositionDict.Count - 1)
            return;

        lerpSequence?.Kill();
        lerpSequence = DOTween.Sequence();
        lerpSequence.Append(content.DOAnchorPosX(itemSnapPositionDict[index], lerpTime));
        lerpSequence.AppendCallback(() =>
        {
            OnSnap?.Invoke(index);
            pageIndicatorController.Select(CurrentItemIndex);
        });

        CurrentItemIndex = index;

        if (!lerpSequence.IsComplete())
            return;

        snapIndex = index;
    }

    public void SnapVerticalTo(int index)
    {
        if (index == snapIndex)
            return;

        if (index < 0 || index > itemSnapPositionDict.Count - 1)
            return;

        lerpSequence?.Kill();
        lerpSequence = DOTween.Sequence();
        lerpSequence.Append(content.DOAnchorPosY(itemSnapPositionDict[index], lerpTime));
        lerpSequence.AppendCallback(() =>
        {
            OnSnap?.Invoke(index);
            pageIndicatorController.Select(CurrentItemIndex);
        });

        CurrentItemIndex = index;

        if (!lerpSequence.IsComplete())
            return;

        snapIndex = index;
    }

    private bool ValidateContentPivot()
    {
        if (horizontal)
        {
            if (content.pivot.x != 1 && content.pivot.x != 0)
            {
                Log.Warning($"Content pivot.x must be at '0' or '1', current pivot is {content.pivot.x}");
                return false;
            }

            isScrollInvert = content.pivot.x == 1;
            content.pivot = new Vector2(0, content.pivot.y);

            return true;
        }
        else if (vertical)
        {
            if (content.pivot.y != 1 && content.pivot.y != 0)
            {
                Log.Warning($"Content pivot.y must be at '0' or '1', current pivot is {content.pivot.y}");
                return false;
            }

            isScrollInvert = content.pivot.y == 0;
            content.pivot = new Vector2(content.pivot.x, 1);

            return true;
        }

        return false;
    }

    private void CheckInvert()
    {
        if (!isScrollInvert)
            return;

        if (horizontal)
            SnapHorizontalTo(itemList.Count - 1);
        else if (vertical)
            SnapVerticalTo(itemList.Count - 1);
    }
}