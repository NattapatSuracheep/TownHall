using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    private static float duration = 0.15f;
    private static bool isPlay = false;

    public static void SetText(this Button button, string text)
    {
        button.GetComponentInChildren<TMP_Text>().text = text;
    }

    public static void OnClickAnimation(this Button button, Action onClick = null, float value = 0.2f, bool isWaitForAnimation = false)
    {
        var trans = button.transform;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (isPlay)
                return;

            isPlay = true;
            if (!isWaitForAnimation)
            {
                try
                {
                    onClick?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                isPlay = false;
            }

            var sequence = DOTween.Sequence();
            sequence.Append(button.image.rectTransform.DOPunchScale(new Vector3(value, value, value), duration));
            sequence.AppendCallback(() =>
            {
                if (!isWaitForAnimation)
                    return;

                try
                {
                    onClick?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                isPlay = false;
            });
        });
    }

    public static void OnClickAnimation(this Button button, Action onClick)
    {
        OnClickAnimation(button, onClick, 0.2f, false);
    }

    public static void OnClickAnimation(this Button button, Func<UniTask> onClick = null, float value = 0.2f, bool isWaitForAnimation = false)
    {
        var trans = button.transform;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (isPlay)
                return;

            isPlay = true;
            if (!isWaitForAnimation)
            {
                try
                {
                    onClick?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                isPlay = false;
            }

            var sequence = DOTween.Sequence();
            sequence.Append(button.image.rectTransform.DOPunchScale(new Vector3(value, value, value), duration));
            sequence.AppendCallback(() =>
            {
                if (!isWaitForAnimation)
                    return;

                try
                {
                    onClick?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                isPlay = false;
            });
        });
    }

    public static void OnClickAnimation(this Button button, Func<UniTask> onClick)
    {
        OnClickAnimation(button, onClick, 0.2f, false);
    }
}