using UnityEngine;
using UnityEngine.UI;

public class PotionBar : MonoBehaviour
{
    [Header("Potion Settings")]
    [SerializeField] private Image potionFillImage;
    [SerializeField] private Canvas potionCanvas;
    [SerializeField] private int maxPotions = 3;
    [SerializeField] private bool hideWhenEmpty = false;
    [SerializeField] private bool alignWithCamera = false;

    private VidaPlayer player;

    private void Awake()
    {
        player = GetComponentInParent<VidaPlayer>();
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }
    }

    private bool ValidateReferences()
    {
        if (player == null)
        {
            Debug.LogError("PotionBar: Missing VidaPlayer reference!");
            return false;
        }

        if (potionFillImage == null || potionCanvas == null)
        {
            Debug.LogError("PotionBar: Missing potion references!");
            return false;
        }
        return true;
    }

    private void Update()
    {
        if (alignWithCamera && Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        UpdatePotionDisplay();
    }

    private void UpdatePotionDisplay()
    {
        float fillAmount = (float)player.healthPotionUsage / maxPotions;
        potionFillImage.fillAmount = fillAmount;
        potionCanvas.gameObject.SetActive(!(hideWhenEmpty && fillAmount <= 0));
    }
}