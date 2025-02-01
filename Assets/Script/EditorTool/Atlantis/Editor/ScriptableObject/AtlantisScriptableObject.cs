using UnityEngine;

[CreateAssetMenu(fileName = "AtlantisBehaviorSetting", menuName = "Atlantis/AtlantisBehaviorSetting")]
public class AtlantisScriptableObject : ScriptableObject
{
    public bool SelectToPack = true;
    public AtlantisValidationController.DuplicateOperation DuplicateOperation = AtlantisValidationController.DuplicateOperation.Skip;
}