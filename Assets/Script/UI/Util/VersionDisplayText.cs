using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField, ReadOnly] private TMP_Text textDisplay;

    private void OnValidate()
    {
        if (textDisplay == null)
            textDisplay = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        textDisplay.text = $"ver.{Application.version}";
    }
}