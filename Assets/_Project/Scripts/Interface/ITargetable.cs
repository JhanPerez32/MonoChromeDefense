using UnityEngine;

public interface ITargetable
{
    Transform GetTransform();
    FactionScriptable GetFaction();
    bool IsActive(); 
}
