using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TownHallPanel : MonoBehaviour
{
    [SerializeField] private ScrollRectSnap scrollSnap;
    [SerializeField] private AssetsPool pool;
    [SerializeField] private AssetReferenceComponent<TownHallObj> objAssetRef;

    private TownHallPrototypeData townHallPrototypeData => GameManager.Instance.GameDataManager.TownHallPrototypeData;
    private InputManager inputManager => GameManager.Instance.InputManager;
    private Vector2 startClickPos;

    public async UniTask InitializeAsync()
    {
        await pool.CreatePool(objAssetRef);

        for (var i = 0; i < townHallPrototypeData.DataList.Count; i++)
        {
            var data = townHallPrototypeData.DataList.ElementAt(i);
            var a = pool.Get<TownHallObj>(scrollSnap.content);
            a.Initialize(data.Key,
            new
            (
                data.Value,
                i == 0 ? true : false,
                i == townHallPrototypeData.DataList.Count - 1 ? true : false
            ));
        }

        await UniTask.NextFrame();
        await scrollSnap.RefreshAsync();

        inputManager.OnUiLeftClickStartedEvent += StoreStartLeftClick;
        inputManager.OnUiLeftClickCanceledEvent += HandleClick;
    }

    private void StoreStartLeftClick()
    {
        startClickPos = inputManager.CurrentPointPosition;
    }

    private void HandleClick()
    {
        var pos = inputManager.CurrentPointPosition;
        if (startClickPos != pos)
            return;

        var height = Screen.height;
        var halfHeight = height * 0.5f;

        if (pos.y > halfHeight)
            ScrollUp();
        else
            ScrollDown();
    }

    private void ScrollUp()
    {
        scrollSnap.SnapVerticalTo(scrollSnap.CurrentItemIndex - 1);
    }

    private void ScrollDown()
    {
        scrollSnap.SnapVerticalTo(scrollSnap.CurrentItemIndex + 1);
    }
}