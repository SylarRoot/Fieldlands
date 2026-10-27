using Enemy;
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] public int currentHealth;
    [SerializeField] private int healthAmount;
    [SerializeField] public int healthPotionUsage;

    // Referència a l'efecte de partícules de curació
    [SerializeField] private ParticleSystem healingParticleEffect;

    private float healthPotionCD = 5f;
    private float timer = 0f;

    private Animator animator;
    private Player_Controler controls;

    public const string Die = "Die";
    public const string PotionDrink = "PotionDrink";
    public const string Move = "Move";
    private string currentAnimationState;

    private DateTime sessionStartTime;
    private string username;

    //////
    private int fireEnemies;
    private int iceEnemies;
    private int lightningEnemies;
    private int meleeFire;
    private int meleeIce;
    private int meleeLightning;
    private int projectileFire;
    private int projectileIce;
    private int projectileLightning;
    private int ability2Fire;
    private int ability2Ice;
    private int ability2Lightning;
    private int Sword;
    //////

    void Awake()
    {
        sessionStartTime = DateTime.UtcNow;
        animator = GetComponent<Animator>();
        controls = new Player_Controler();
        InitializeHealth();

        controls.Player.Heal.performed += ctx => Heal();

        if (healingParticleEffect != null)
            healingParticleEffect.Stop();

        username = PlayerPrefs.GetString("LoggedInUser", "Offline");
        if (PlayerPrefs.GetInt("IsOnline") == 1 && MongoDBManager.Instance != null)
            MongoDBManager.Instance.InitializeUserStats(username);
    }

    void Update() => HealthPotionColdown();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void InitializeHealth() => currentHealth = maxHealth;

    private void OnCollisionEnter(Collision collision)
    {
        if (currentHealth <= 0) return;

        if (collision.gameObject.CompareTag("Axe"))
        {
            var enemy = collision.gameObject.GetComponent<EnemyAi_melee>();
            if (enemy != null)
                TakeDamage(enemy.attackDamage);
        }
        else if (collision.gameObject.CompareTag("Boss_Attack"))
        {
            var boss = collision.gameObject.GetComponent<EnemyAi_Boss>();
            if (boss != null)
                TakeDamage(boss.attackDamage);
        }
    }

    public void Heal()
    {
        if (currentHealth <= 0 || currentHealth >= maxHealth) return;
        if (healthPotionUsage <= 0)
        {
            Debug.Log("No tens més pocions de salut disponibles.");
            return;
        }

        healthPotionUsage--;
        ChangeAnimationState(PotionDrink);
        // Esperem que acabi l'animació abans de començar la curació
        StartCoroutine(HealAfterAnimation(healthAmount, 2f));
    }

    // Córrutina que gestiona l'espera de l'animació, la curació progressiva i el retorn a Move
    private IEnumerator HealAfterAnimation(int totalHeal, float duration)
    {
        // Temps d'animació de la poció
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length + 1 ;
        yield return new WaitForSeconds(animLength);

        // Comença curació progressiva
        yield return StartCoroutine(HealOverTime(totalHeal, duration));

        // Retorna a l'estat Move
        ChangeAnimationState(Move);
    }

    // Córrutina per curar de manera progressiva
    private IEnumerator HealOverTime(int totalHeal, float duration)
    {
        int steps = 20;
        float healPerStep = (float)totalHeal / steps;
        float delayPerStep = duration / steps;
        float healed = 0f;

        // Activa l'efecte de partícules
        if (healingParticleEffect != null)
            healingParticleEffect.Play();
        else
            Debug.LogWarning("Efecte de partícules no assignat!");

        while (healed < totalHeal && currentHealth < maxHealth)
        {
            float amount = Mathf.Min(healPerStep, totalHeal - healed);
            currentHealth = Mathf.Min(currentHealth + Mathf.RoundToInt(amount), maxHealth);
            healed += amount;
            Debug.Log($"Curant {amount:F1}. Vida actual: {currentHealth}/{maxHealth}");
            yield return new WaitForSeconds(delayPerStep);
        }

        // Deixa un petit marge i atura l'efecte
        yield return new WaitForSeconds(0.5f);
        if (healingParticleEffect != null)
            healingParticleEffect.Stop();
    }

    public void HealthPotionColdown()
    {
        if (healthPotionUsage < 4)
            healthPotionCD += Time.deltaTime;

        if (healthPotionCD >= 10f)
        {
            healthPotionUsage++;
            timer = healthPotionCD;
            healthPotionCD = 0f;
            Debug.Log($"Poció de salut recuperada. Disponibles: {healthPotionUsage}");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damageAmount;
        Debug.Log($"Dany rebut: {damageAmount}. Vida restant: {currentHealth}");
        if (currentHealth <= 0)
        {
            SaveSessionData();
            die();
        }
    }

    private void SaveSessionData()
    {
        var endTime = DateTime.UtcNow;
        var duration = (endTime - sessionStartTime).TotalSeconds;
        PlayerPrefs.SetFloat("CurrentSessionDuration", (float)duration);
        if (PlayerPrefs.GetInt("IsOnline") == 1 && MongoDBManager.Instance != null)
            MongoDBManager.Instance.UpdateSessionDuration(username, duration);
        Debug.Log($"Temps de sessió guardat: {duration}s");
    }

    public void die()
    {
        ChangeAnimationState(Die);
        StartCoroutine(LoadGameOverWithDelay(0.5f));

        if (PlayerPrefs.GetInt("IsOnline") == 1 && MongoDBManager.Instance != null)
        {
            // Carrega i desa estadístiques
            MongoDBManager.Instance.SaveCurrentGameStats(
                PlayerPrefs.GetFloat("CurrentSessionDuration"),
                PlayerPrefs.GetInt("CurrentFireEnemies"),
                PlayerPrefs.GetInt("CurrentIceEnemies"),
                PlayerPrefs.GetInt("CurrentLightningEnemies"),
                PlayerPrefs.GetInt("CurrentMeleeFireEnemies"),
                PlayerPrefs.GetInt("CurrentMeleeIceEnemies"),
                PlayerPrefs.GetInt("CurrentMeleeLightningEnemies"),
                PlayerPrefs.GetInt("CurrentProjectileFire"),
                PlayerPrefs.GetInt("CurrentProjectileIce"),
                PlayerPrefs.GetInt("CurrentProjectileLightning"),
                PlayerPrefs.GetInt("CurrentAbility2Fire"),
                PlayerPrefs.GetInt("CurrentAbility2Ice"),
                PlayerPrefs.GetInt("CurrentAbility2Lightning"),
                PlayerPrefs.GetInt("CurrentSword"));
            Debug.Log("Dades de la partida guardades.");
        }
    }

    private IEnumerator LoadGameOverWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOver");
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        if (animator != null && animator.HasState(0, Animator.StringToHash(newState)))
        {
            currentAnimationState = newState;
            animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
        }
        else
        {
            Debug.LogError($"Animation state '{newState}' no trobat.");
        }
    }
}
