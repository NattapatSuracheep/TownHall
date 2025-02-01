using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetReferenceComponent<TComponent> : AssetReferenceGameObject where TComponent : Component
{
    public AssetReferenceComponent(string guid) : base(guid)
    {
    }
#if UNITY_EDITOR
    public override bool ValidateAsset(string mainAssetPath)
    {
        return AssetDatabase.LoadAssetAtPath(mainAssetPath, typeof(TComponent)) != null;
    }
#endif
}