using UnityEngine;

public class TurretShopUI : MonoBehaviour
{
    [Header("Available Turrets")]
    [SerializeField] private TurretData[] availableTurrets;

    [Header("References")]
    [SerializeField] private Transform contentParent;

    [SerializeField] private TurretShopItemUI shopItemPrefab;

    [SerializeField] private TowerPlacementSystem towerPlacementSystem;

    private void Start()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (TurretData turret in availableTurrets)
        {
            TurretShopItemUI turretShopItemUI = Instantiate(shopItemPrefab, contentParent);

            turretShopItemUI.Initialize(turret, towerPlacementSystem);
        }
    }
}
