using UnityEngine;

public class ToggleEventListener : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private BoolEvent toggleEvent;

    [Header("Target GameObject")]
    [SerializeField] private GameObject target;

    private void OnEnable()
    {
        toggleEvent.Register(OnToggleEvent);
    }

    private void OnDisable()
    {
        toggleEvent.Unregister(OnToggleEvent);
    }

    private void OnToggleEvent(bool value)
    {
        if (target)
        {
            target.SetActive(value);
        }
    }
}
