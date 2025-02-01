using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderAndInputField : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputFieldSlider;

    public event Action<float> OnValueChanged;

    public void ChangeValue(float value)
    {
        slider.value = value;
    }

    public void ChangeValueWithOutNotify(float value)
    {
        slider.SetValueWithoutNotify(value);
        inputFieldSlider.text = value.ToString("N1");
    }

    public void Initialize(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        inputFieldSlider.onEndEdit.AddListener(OnInputEndEdit);
        slider.onValueChanged.AddListener(OnSliderValueChange);

        slider.value = value;
    }

    private void OnInputEndEdit(string input)
    {
        var value = float.Parse(input);
        slider.value = value;
        inputFieldSlider.text = slider.value.ToString("N1");
    }

    private void OnSliderValueChange(float input)
    {
        inputFieldSlider.text = input.ToString("N1");

        OnValueChanged?.Invoke(input);
    }
}
