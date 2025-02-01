using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    private const float sliderAnimateDuration = 0.2f;

    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Slider slider;
    [SerializeField] private CanvasGroup sliderCanvasGroup;
    [SerializeField] private Image loadingIcon;

    private bool isDisplayLoadingBar;

    private Sequence loadingIconSequence;

    private void OnDestroy()
    {
        loadingIconSequence?.Kill();
    }

    public void InitializeWithLoadingBar(string info, float maxValue = 1f)
    {
        SetInfoText(info);
        SetLoadingInfoIconSequence();
        SetLoadingBarData(maxValue);

        isDisplayLoadingBar = true;
    }

    public void InitializeWithOutLoadingBar(string info)
    {
        sliderCanvasGroup.Hide();

        SetInfoText(info);
        SetLoadingInfoIconSequence();

        isDisplayLoadingBar = false;
    }

    private void SetInfoText(string info)
    {
        infoText.text = info;
    }

    private void SetLoadingInfoIconSequence()
    {
        loadingIconSequence = DOTween.Sequence();
        loadingIconSequence.Append(loadingIcon.transform.DOLocalRotate(new Vector3(0, 360 * 2, 0), 1.5f, RotateMode.FastBeyond360)).SetEase(Ease.InOutQuad);
        loadingIconSequence.AppendInterval(0.2f);
        loadingIconSequence.SetLoops(-1);
    }

    private void SetLoadingBarData(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = 0;

        sliderCanvasGroup.Show();
    }

    public void UpdateData(string info, float value)
    {
        UpdateInfoText(info);
        UpdateSliderValue(value);
    }

    private void UpdateInfoText(string info)
    {
        infoText.text = info;
    }

    private void UpdateSliderValue(float value)
    {
        if (!isDisplayLoadingBar)
            return;

        if (value == -1)
            value = slider.value;

        slider.DOValue(value, sliderAnimateDuration);
    }
}
