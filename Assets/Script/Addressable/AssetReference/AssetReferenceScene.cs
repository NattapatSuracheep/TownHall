using System;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetReferenceScene : AssetReference
{
    public AssetReferenceScene(string guid) : base(guid)
    {
    }

    public override bool ValidateAsset(string path)
    {
        return path.EndsWith(".unity");
    }
}