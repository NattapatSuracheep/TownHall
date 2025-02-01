using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownHallObj_2 : MonoBehaviour
{
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private LayoutElement leftLayoutElement;
    [SerializeField] private LayoutElement rightLayoutElement;

    public void Initialize(int index, string topic, string[] messages)
    {
        leftLayoutElement.preferredWidth = Screen.width * 0.4f;
        rightLayoutElement.preferredWidth = Screen.width * 0.4f;

        var tex = new StringBuilder();
        tex.Append($"<b>{topic}</b>\n");

        foreach (var item in messages)
            tex.Append($"- {item}\n");

        if (index % 2 == 0)
            leftText.text = tex.ToString();
        else
            rightText.text = tex.ToString();
    }
}
