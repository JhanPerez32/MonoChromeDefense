using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFactionData", menuName = "ScriptableObjects/Faction/FactionData")]
public class FactionScriptable : ScriptableObject
{
    [Header("Stable ID (DO NOT CHANGE AFTER RELEASE)")]
    [SerializeField] private string id;

    private string ID => id;

    [Header("Display")]
    public string displayName;

    [Header("Relationships")]
    [SerializeField] private List<FactionScriptable> hostileTo = new();

    public bool IsHostile(FactionScriptable hostileFaction)
    {
        return hostileFaction && hostileTo.Contains(hostileFaction);
    }

    public bool IsFriendly(FactionScriptable friendlyFaction)
    {
        return friendlyFaction != null && ID == friendlyFaction.ID;
    }
}
