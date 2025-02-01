using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(ScrollRect))]
public class TextScrollUi : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private const float delayBeforeScroll = 2;
    private const float delayBeforeReset = 2;
    private const float scrollDuration = 1.5f;

    [SerializeField, ReadOnly] private ScrollRect scrollRect;
    [SerializeField, ReadOnly] private TMP_Text tMP_Text;
    [SerializeField] private bool isHorizontalTextAlignCenter = false;

    private RectTransform contentTransform => scrollRect.content;
    private RectTransform textTransform => tMP_Text.rectTransform;

    private Sequence sequence;
    private Sequence verticalScrollSequence;
    private Sequence horizontalScrollSequence;

    private bool isHaveToScroll;
    private bool isUserManualDrag;

    public TMP_Text Tmp_Text => tMP_Text;

    private void OnValidate()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        if (tMP_Text == null)
            tMP_Text = GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        sequence?.Kill();
        KillSequences();
    }

    private void Start()
    {
        ScrollSequence();
    }

    public void SetText(string message)
    {
        tMP_Text.text = message;
        UpdateScroll().Forget();
    }

    public async UniTask UpdateScroll()
    {
        //wait 1 frame for auto size text to adjust their size
        await UniTask.NextFrame();

        var size = tMP_Text.fontSize; //copy adjusted size
        tMP_Text.fontSizeMax = size; //set max size to adjusted size
        /*
        NOTE: if max size is bigger than actual text size, it will leave the empty space at the end of the text

        Solution
        1. set new max size to the size that can actually use for all ui (impossible in this project :P)
        2. set new max size to the size that auto size give to this text
        3. store size that auto size give to this text, disable auto size, set the font size with the store size
        */

        if (scrollRect.horizontal)
        {
            isHaveToScroll = textTransform.rect.width > contentTransform.rect.width;
            if (!isHaveToScroll)
                return;

            if (isHorizontalTextAlignCenter)
            {
                contentTransform.pivot = new Vector2(0.5f, 1);
                return;
            }

            contentTransform.pivot = new Vector2(0, 1);
        }
        else if (scrollRect.vertical)
        {
            isHaveToScroll = textTransform.rect.height > contentTransform.rect.height;
            if (!isHaveToScroll)
                return;

            contentTransform.pivot = new Vector2(0, 1);
        }

        ResetScrollNormalizedPosition();
    }

    private void ScrollSequence()
    {
        sequence = DOTween.Sequence();
        sequence.AppendInterval(delayBeforeScroll);
        sequence.AppendCallback(Scroll);
        sequence.AppendInterval(scrollDuration);
        sequence.AppendInterval(delayBeforeReset);
        sequence.AppendCallback(Reset);
        sequence.SetLoops(-1);
    }

    private void Scroll()
    {
        if (isUserManualDrag)
            return;

        if (scrollRect.vertical)
            VerticalScroll();
        else if (scrollRect.horizontal)
            HorizontalScroll();
    }

    private void VerticalScroll()
    {
        verticalScrollSequence = DOTween.Sequence();
        verticalScrollSequence.Append(scrollRect.DOVerticalNormalizedPos(1, scrollDuration));
    }

    private void HorizontalScroll()
    {
        horizontalScrollSequence = DOTween.Sequence();
        horizontalScrollSequence.Append(scrollRect.DOHorizontalNormalizedPos(1, scrollDuration));
    }

    private void Reset()
    {
        if (isUserManualDrag)
            return;

        ResetScrollNormalizedPosition();
    }

    private void ResetScrollNormalizedPosition()
    {
        KillSequences();

        if (scrollRect.vertical)
            scrollRect.verticalNormalizedPosition = 0;
        else if (scrollRect.horizontal)
            scrollRect.horizontalNormalizedPosition = 0;
    }

    private void KillSequences()
    {
        verticalScrollSequence?.Kill();
        horizontalScrollSequence?.Kill();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isUserManualDrag = true;

        KillSequences();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isUserManualDrag = true;

        KillSequences();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isUserManualDrag = false;
    }
}