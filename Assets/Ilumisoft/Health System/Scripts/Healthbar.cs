using Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Canvas canvas;
        [SerializeField] private bool hideWhenEmpty = false;
        [SerializeField] private bool alignWithCamera = false;

        private Enemy_Vida enemy;
        private float maxHealth;

        private void Awake()
        {
            enemy = GetComponentInParent<Enemy_Vida>();
            if (enemy == null || fillImage == null || canvas == null)
            {
                Debug.LogError("Healthbar: Missing references!");
                enabled = false;
                return;
            }
            maxHealth = enemy.maxHealth;
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
            float fillAmount = enemy.currentHealth / maxHealth;
            fillImage.fillAmount = fillAmount;
            canvas.gameObject.SetActive(!(hideWhenEmpty && fillAmount <= 0));
        }
    }
}
