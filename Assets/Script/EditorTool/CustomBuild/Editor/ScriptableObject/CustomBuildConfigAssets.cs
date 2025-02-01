using UnityEngine;

[CreateAssetMenu(fileName = "CustomBuildConfigAssets", menuName = "Build/CustomBuildConfigAssets")]
public class CustomBuildConfigAssets : ScriptableObject
{
    [ReadOnly] public string buildFolder = "Builds";
    [ReadOnly] public string buildName;
    [ReadOnly] public string buildVersion;
    [ReadOnly] public string buildNumber;
    [ReadOnly] public string fileType;
    [ReadOnly] public string buildPath;
    public bool isDevBuild;
    public bool isGoogleStoreBuild;

    [TextArea(3, 10)]
    public string customBuildConfigData;
}