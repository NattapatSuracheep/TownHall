using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetsPool : MonoBehaviour
{
    private GameObject resourceObject;
    private AssetReferenceGameObject assetReference;
    private GameObject originalPoolObj;
    private List<int> getObjectIdList = new();
    private Queue<GameObject> pool;
    private List<GameObject> allObjInPoolList;

    private void OnDestroy()
    {
        if (pool != null)
        {
            Log.Call();

            foreach (var item in allObjInPoolList)
                Destroy(item);

            Destroy(originalPoolObj);
            pool.Clear();
            allObjInPoolList.Clear();
            getObjectIdList.Clear();
            resourceObject = null;

            if (resourceObject != null)
                Resources.UnloadUnusedAssets();
            else if (assetReference != null)
                AddressableManager.ReleaseGameObject(assetReference);
        }
    }

    public async UniTask CreatePool(AssetReference assetReference, int preLoadSize = 1)
    {
        this.assetReference = (AssetReferenceGameObject)assetReference;
        var obj = await AddressableManager.InstantiateGameObjectAsync((AssetReferenceGameObject)assetReference);
        StoreOriginalPoolObj(obj);
        AssignObjectToPool(preLoadSize);
    }

    public void CreatePool(GameObject poolObject, int preLoadSize = 1)
    {
        var obj = GameObject.Instantiate(poolObject);
        StoreOriginalPoolObj(obj);
        AssignObjectToPool(preLoadSize);
    }

    public void CreatePool(string poolObjectResourcesPath, int preLoadSize = 1)
    {
        resourceObject = Resources.Load<GameObject>(poolObjectResourcesPath);
        var obj = GameObject.Instantiate(resourceObject);
        StoreOriginalPoolObj(obj);
        AssignObjectToPool(preLoadSize);
    }

    private void AssignObjectToPool(int size)
    {
        pool = new();
        allObjInPoolList = new();
        for (var i = 0; i < size; i++)
        {
            var cloneObj = Clone();
            pool.Enqueue(cloneObj);
            allObjInPoolList.Add(cloneObj);
        }
    }

    private void StoreOriginalPoolObj(GameObject originalObj)
    {
        originalObj.name = originalObj.name.Replace("(Clone)", " - (Original)");
        originalObj.transform.SetParent(this.transform, false);
        originalObj.SetActive(false);
        originalPoolObj = originalObj;
    }

    private GameObject Clone()
    {
        var cloneObj = GameObject.Instantiate(originalPoolObj);
        cloneObj.SetActive(false);
        cloneObj.transform.SetParent(this.transform, false);
        cloneObj.name = cloneObj.name.Replace("(Original)(Clone)", "(Clone)");
        return cloneObj;
    }

    public T Get<T>(Transform parent = null)
    {
        return Get(parent).GetComponent<T>();
    }

    public GameObject Get(Transform parent = null)
    {
        if (pool == null)
        {
            Debug.LogWarning($"Pool is not created");
            return default;
        }

        if (pool.Count == 0)
            pool.Enqueue(Clone());

        var obj = pool.Dequeue();
        obj.SetActive(true);
        parent = parent == null ? this.transform : parent;
        obj.transform.SetParent(parent, false);
        getObjectIdList.Add(obj.GetInstanceID());
        return obj;
    }

    public void Return(GameObject gameObject)
    {
        if (pool == null)
        {
            Debug.LogWarning($"Pool is not created");
        }

        if (getObjectIdList.Contains(gameObject.GetInstanceID()))
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(this.transform, false);
            pool.Enqueue(gameObject);
            return;
        }

        Debug.LogWarning($"Can't return object to this pool / getObjectIdList not contains object id: {gameObject.GetInstanceID()}");
    }
}