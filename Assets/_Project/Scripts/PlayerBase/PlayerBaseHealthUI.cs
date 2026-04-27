using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBaseHealthUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text baseNumberText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CanvasGroup canvasGroup;

    private PlayerBase _linkedBase;
    private const string BASE_NUMBER = "Base: ";

    public void Initialize(PlayerBase playerBase, int index)
    {
        _linkedBase = playerBase;

        baseNumberText.text = BASE_NUMBER + (index + 1);
    }

    public void UpdateHealth(float current, float max)
    {
        healthSlider.value = current / max;
    }

    public void SetDeadVisual(bool isDead)
    {
        if (!canvasGroup) return;

        canvasGroup.alpha = isDead ? 0.4f : 1f;
        canvasGroup.interactable = !isDead;
        canvasGroup.blocksRaycasts = !isDead;
    }
}
