using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TownHallObj_2 : MonoBehaviour
{
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private LayoutElement leftLayoutElement;
    [SerializeField] private LayoutElement rightLayoutElement;
    [SerializeField] private TMP_Text leftImgTopic;
    [SerializeField] private Image leftImg;
    [SerializeField] private TMP_Text rightImgTopic;
    [SerializeField] private Image rightImg;

    public async UniTask Initialize(int index, string topic, string[] messages)
    {
        if (await HandleImage(index, topic, messages) == true)
            return;

        var value = Screen.width * 0.45f; //max width
        leftLayoutElement.preferredWidth = value;
        rightLayoutElement.preferredWidth = value;

        var tex = new StringBuilder();
        tex.Append($"<b>{topic}</b>\n");

        foreach (var item in messages)
            tex.Append($"- {item}\n");

        if (index % 2 == 0)
            leftText.text = tex.ToString();
        else
            rightText.text = tex.ToString();
    }

    private async UniTask<bool> HandleImage(int index, string topic, string[] message)
    {
        var msg = message[0];
        if (msg.Contains("image=") == false)
            return false;

        var startIndex = msg.IndexOf("image=") + "image=".Length;
        var imageUrl = string.Format(msg.Substring(startIndex), GameManager.CurrentURL);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        await request.SendWebRequest();
        var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        var img = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        if (index % 2 == 0)
        {
            leftImgTopic.text = $"<b>{topic}</b>";
            leftImg.sprite = img;
            leftImg.gameObject.SetActive(true);
        }
        else
        {
            rightImgTopic.text = $"<b>{topic}</b>";
            rightImg.sprite = img;
            rightImg.gameObject.SetActive(true);
        }

        return true;
    }
}
