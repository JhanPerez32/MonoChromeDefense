using UnityEngine;

[CreateAssetMenu(fileName = "New TabType", menuName = "ScriptableObjects/Tabs/TabType")]
public class TabTypeScriptable : ScriptableObject
{
    [SerializeField] private string displayName;
    
    public string DisplayName => displayName;
}
