using UnityEditor;

public class AtlantisBehaviorController : AtlantisComponent
{
    private const string behaviorSettingPath = "Assets/Script/EditorTool/Atlantis/Editor/AtlantisBehaviorSetting.asset";

    private AtlantisScriptableObject behaviorSetting;
    public bool DefaultSelectToPack => behaviorSetting.SelectToPack;
    public AtlantisValidationController.DuplicateOperation DefaultDuplicateOperation => behaviorSetting.DuplicateOperation;

    public override void Initialize(AtlantisAtlasPacker atlantis)
    {
        base.Initialize(atlantis);

        if (behaviorSetting == null)
            LoadBuildConfigAssets();
    }

    public void GUI()
    {
        behaviorSetting = (AtlantisScriptableObject)EditorGUILayout.ObjectField("Behavior Setting", behaviorSetting, typeof(AtlantisScriptableObject), false);
    }

    private void LoadBuildConfigAssets()
    {
        behaviorSetting = AssetDatabase.LoadAssetAtPath<AtlantisScriptableObject>(behaviorSettingPath);
    }
}