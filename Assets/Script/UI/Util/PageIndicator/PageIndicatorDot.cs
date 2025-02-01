using UnityEngine;
using UnityEngine.UI;

public class PageListDot : BasePageIndicatorSlot
{
    [SerializeField] private Image unSelectImage;
    [SerializeField] private Image selectImage;

    public void Initialize()
    {
        Unselect();
    }

    public override void Select()
    {
        base.Select();

        unSelectImage.gameObject.SetActive(false);
        selectImage.gameObject.SetActive(true);
    }

    public override void Unselect()
    {
        base.Unselect();

        unSelectImage.gameObject.SetActive(true);
        selectImage.gameObject.SetActive(false);
    }
}