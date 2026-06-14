using UnityEngine;

public class TowerPlacementSystem : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private BuildPlatformClickedEvent buildPlatformClickedEvent;
    
    private TurretData _selectedTurret;

    private void OnEnable()
    {
        buildPlatformClickedEvent.Register(HandlePlatformClicked);
    }

    private void OnDisable()
    {
        buildPlatformClickedEvent.Unregister(HandlePlatformClicked);
    }

    private void HandlePlatformClicked(BuildPlatform platform)
    {
        PlaceTurret(platform);
    }

    public void SelectTurret(TurretData turretData)
    {
        _selectedTurret = turretData;

        Debug.Log($"Selected: {turretData.turretName}");
    }

    private void PlaceTurret(BuildPlatform platform)
    {
        if (platform.IsOccupied) return;

        if (!_selectedTurret)
        {
            Debug.LogWarning("No turret selected.");
            return;
        }

        GameObject turretObject = Instantiate(_selectedTurret.turretPrefab);

        Turret turret = turretObject.GetComponent<Turret>();

        AlignTurret(turret, platform.TurretAnchor);

        platform.SetTurret(turret);

        Debug.Log($"Placed {_selectedTurret.turretName}");
    }

    private void AlignTurret(Turret turret, Transform transformAnchor)
    {
        Transform center = turret.CenterPoint;

        Vector3 offset = turret.transform.position - center.position;

        turret.transform.position = transformAnchor.position + offset;

        turret.transform.rotation = transformAnchor.rotation;
    }
}
