using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "ScriptableObjects/Turret/TurretData")]
public class TurretData : ScriptableObject
{
    [Header("UI")]
    public string turretName;
    public Sprite turretIcon;

    [Header("Prefab")]
    public GameObject turretPrefab;

    [Header("Stats")]
    public int turretCost;
}
