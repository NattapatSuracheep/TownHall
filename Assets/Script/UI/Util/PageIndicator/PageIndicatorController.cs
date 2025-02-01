using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PageIndicatorController : MonoBehaviour
{
    [SerializeField] private AssetReferenceComponent<BasePageIndicatorSlot> pageListSlotAssetRef;
    [SerializeField] private AssetsPool pool;
    [SerializeField] private Transform container;

    private int currentSelectPage = -1;

    private List<BasePageIndicatorSlot> pageListSlots = new();
    private bool isInitialize;

    public async UniTask InitializeAsync(int pageAmount)
    {
        if (isInitialize)
        {
            Reload(pageAmount);
            return;
        }

        await pool.CreatePool(pageListSlotAssetRef);

        for (var i = 0; i < pageAmount; i++)
        {
            var slot = pool.Get<BasePageIndicatorSlot>(container);
            pageListSlots.Add(slot);
        }

        Select(0);

        isInitialize = true;
    }

    public void Reload(int pageAmount, int selectPageIndex = 0)
    {
        if (!isInitialize)
        {
            Debug.LogWarning($"Initialize first before reload");
            return;
        }
        else if (pageListSlots.Count == pageAmount)
            return;

        for (var i = 0; i < pageListSlots.Count; i++)
        {
            var slot = pageListSlots[i];
            pool.Return(slot.gameObject);
        }

        pageListSlots.Clear();
        for (var i = 0; i < pageAmount; i++)
        {
            var slot = pool.Get<BasePageIndicatorSlot>(container);
            pageListSlots.Add(slot);
        }

        currentSelectPage = -1;
        Select(selectPageIndex);
    }

    public void Select(int selectPage)
    {
        if (pageListSlots.Count == 0)
        {
            return;
        }
        else if (currentSelectPage == selectPage)
        {
            return;
        }
        else if (selectPage == -1)
        {
            Debug.LogWarning($"Select page can't be -1");
            return;
        }

        if (currentSelectPage != -1)
            pageListSlots[currentSelectPage].Unselect();

        pageListSlots[selectPage].Select();

        currentSelectPage = selectPage;
    }
}