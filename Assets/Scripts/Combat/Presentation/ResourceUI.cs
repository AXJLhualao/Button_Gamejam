using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable()
    {
        resourceManager.OnGoldChanged += UpdateDisplay;
    }

    private void OnDisable()
    {
        resourceManager.OnGoldChanged -= UpdateDisplay;
    }

    private void Start()
    {
        UpdateDisplay(resourceManager.Gold);
    }

    private void UpdateDisplay(int gold)
    {
        goldText.text = $"Gold: {gold}";
    }
}
