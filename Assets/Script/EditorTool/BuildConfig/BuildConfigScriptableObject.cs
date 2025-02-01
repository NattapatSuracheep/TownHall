using UnityEngine;

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Build/BuildConfig")]
public class BuildConfigScriptableObject : ScriptableObject
{
    public string AddressableHost;
    public string Version;
}