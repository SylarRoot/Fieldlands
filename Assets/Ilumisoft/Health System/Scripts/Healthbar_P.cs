using UnityEngine;
using UnityEngine.UI;

public class Healthbar2 : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool hideWhenEmpty = false;
    [SerializeField] private bool alignWithCamera = false;

    private VidaPlayer player;

    private void Awake()
    {
        player = GetComponentInParent<VidaPlayer>();
    
        UpdateHealthbar();
    }

    private void Update()
    {
        if (alignWithCamera && Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        if (player == null) return;

        float fillAmount = (float)player.currentHealth / player.maxHealth;
        fillImage.fillAmount = fillAmount;

        
        canvas.gameObject.SetActive(!(hideWhenEmpty && fillAmount <= 0));
    }
}