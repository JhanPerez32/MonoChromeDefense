using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretShopItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    [SerializeField] private TMP_Text turretNameText;

    [SerializeField] private TMP_Text costText;

    [SerializeField] private Button button;

    private TurretData _turretData;
    private TowerPlacementSystem _towerPlacementSystem;

    public void Initialize(TurretData data, TowerPlacementSystem placementSystem)
    {
        _turretData = data;
        _towerPlacementSystem = placementSystem;

        iconImage.sprite = data.turretIcon;
        turretNameText.text = data.turretName;
        costText.text = $"${data.turretCost}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        _towerPlacementSystem.SelectTurret(_turretData);
    }
}
