using UnityEngine;

[CreateAssetMenu(fileName = "New PathPreference", menuName = "ScriptableObjects/AI/PathPreference")]
public class PathPreferenceScriptable : ScriptableObject
{
    public string preferenceName;

    [Header("Targeting")]
    public TargetPriorityType targetPriority;

    [Header("Path Behavior")]
    public PathPreferenceTypeEnum pathPreference;

    [Header("Weight")]
    [Range(0f, 1f)]
    public float selectionWeight = 1f;
}
