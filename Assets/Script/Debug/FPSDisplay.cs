using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    float delayUpdate = 0.1f;
    float currentDelay;

    TextMeshProUGUI fps_txt;

    private void Awake()
    {
        fps_txt = GetComponent<TextMeshProUGUI>();
        currentDelay = delayUpdate;
    }

    private void Update()
    {
        if (currentDelay > 0)
        {
            currentDelay -= Time.deltaTime;
        }
        else
        {
            fps_txt.text = ((int)(1f / Time.smoothDeltaTime)).ToString();
            currentDelay = delayUpdate;
        }
    }
}