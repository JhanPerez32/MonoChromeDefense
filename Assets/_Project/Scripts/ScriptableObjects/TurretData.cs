using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "ScriptableObjects/Turret/TurretData")]
public class TurretData : ScriptableObject
{
    [Header("Visual")]
    public GameObject TurretPrefab;
}
