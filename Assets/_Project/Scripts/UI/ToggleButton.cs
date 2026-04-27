using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private BoolEvent boolEvent;

    private bool _isToggled;

    private void Awake()
    {
        toggleButton.onClick.AddListener(ApplyToggle);
    }

    private void ApplyToggle()
    {
        _isToggled = !_isToggled;
        boolEvent?.Raise(_isToggled);
    }
}
