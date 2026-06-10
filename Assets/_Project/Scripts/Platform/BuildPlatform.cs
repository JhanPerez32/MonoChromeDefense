using System;
using UnityEngine;

public class BuildPlatform : MonoBehaviour
{
    public static event Action<BuildPlatform> OnPlatformClicked;

    [SerializeField] private Transform turretAnchor;

    private Turret _currentTurret;

    public Transform TurretAnchor => turretAnchor;
    public bool IsOccupied => _currentTurret != null;

    public void SetTurret(Turret turret)
    {
        _currentTurret = turret;
    }

    private void OnMouseDown()
    {
        OnPlatformClicked?.Invoke(this);
    }
}
