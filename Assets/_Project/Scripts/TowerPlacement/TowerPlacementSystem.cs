using UnityEngine;

public class TowerPlacementSystem : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private BuildPlatformClickedEvent buildPlatformClickedEvent;

    [Header("Selected Tower")]
    [SerializeField] private TurretData selectedTurret;

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
        selectedTurret = turretData;
    }

    private void PlaceTurret(BuildPlatform platform)
    {
        if (platform.IsOccupied) return;

        if (!selectedTurret)
        {
            Debug.LogWarning("No turret selected.");
            return;
        }

        GameObject turretObject = Instantiate(selectedTurret.TurretPrefab);

        Turret turret = turretObject.GetComponent<Turret>();

        AlignTurret(turret, platform.TurretAnchor);

        platform.SetTurret(turret);

        Debug.Log($"Placed {selectedTurret.name}");
    }

    private void AlignTurret(Turret turret, Transform anchor)
    {
        Transform center = turret.CenterPoint;

        Vector3 offset = turret.transform.position - center.position;

        turret.transform.position = anchor.position + offset;

        turret.transform.rotation = anchor.rotation;
    }
}
