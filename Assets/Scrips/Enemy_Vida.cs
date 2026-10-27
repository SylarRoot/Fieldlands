using MongoDB.Driver;
using UnityEngine;

namespace Enemy
{
    public class Enemy_Vida : MonoBehaviour
    {
        [SerializeField] public int maxHealth;
        [SerializeField] public int currentHealth;
        int damage_a;
        int damage;
        int damageMelee;

        public Spawner spawner; // Assignat des de Spawner.cs


        private void Start()
        {
            
            spawner = FindObjectOfType<Spawner>();


            
            currentHealth = maxHealth;
        }
        private void OnCollisionEnter(Collision collision)
        {

            // Verifica si és un objecte que pot fer mal
            if (collision.gameObject.CompareTag("Projectile_Fire") ||
                collision.gameObject.CompareTag("Projectile_Ice") ||
                collision.gameObject.CompareTag("Projectile_Lightning") ||
                collision.gameObject.CompareTag("Ability2_Fire") ||
                collision.gameObject.CompareTag("Sword"))
            {
                PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
                if (playerAttack == null) return;

                // Inicialitzem danys
                damage = 0;
                damage_a = 0;
                damageMelee = 0;

                // Si és atac melee
                if (collision.gameObject.CompareTag("Sword"))
                {
                    damageMelee = playerAttack.Sword_Damage;
                    currentHealth -= damageMelee;
                    Debug.Log($"[Sword] Dany de {collision.gameObject.tag}: {damageMelee} (Salut restant: {currentHealth})");

             if (currentHealth <= 0)
                    {
                        die();
                        Debug.Log("Enemic derrotat (espasa)!");
                    }

                    return;
                }


                else
                {
                    // És un projectil o ability
                    if (collision.gameObject.CompareTag("Ability2_Fire"))
                    {
                        damage_a = playerAttack.Ability_2_Damage;
                    }
                    else
                    {
                        damage = playerAttack.projectileDamage;
                    }

                    // Bonus x2 si l’element coincideix
                    if ((gameObject.CompareTag("Enemy_Fire") && collision.gameObject.CompareTag("Projectile_Fire")) ||
                        (gameObject.CompareTag("Enemy_Melee_Fire") && collision.gameObject.CompareTag("Projectile_Fire")) ||
                        (gameObject.CompareTag("Enemy_Ice") && collision.gameObject.CompareTag("Projectile_Ice")) ||
                        (gameObject.CompareTag("Enemy_Melee_Ice") && collision.gameObject.CompareTag("Projectile_Ice")) ||
                        (gameObject.CompareTag("Enemy_Lightning") && collision.gameObject.CompareTag("Projectile_Lightning")) ||
                        (gameObject.CompareTag("Enemy_Melee_Lightning") && collision.gameObject.CompareTag("Projectile_Lightning")) ||
                        (gameObject.CompareTag("Boss_Fire") && collision.gameObject.CompareTag("Projectile_Fire")) ||
                        (gameObject.CompareTag("Boss_Ice") && collision.gameObject.CompareTag("Projectile_Ice")) ||
                        (gameObject.CompareTag("Boss_Lightning") && collision.gameObject.CompareTag("Projectile_Lightning")))
                    {
                        damage *= 2;
                    }

                    if ((gameObject.CompareTag("Enemy_Fire") && collision.gameObject.CompareTag("Ability2_Fire")) ||
                        (gameObject.CompareTag("Enemy_Melee_Fire") && collision.gameObject.CompareTag("Ability2_Fire")) ||
                        (gameObject.CompareTag("Boss_Fire") && collision.gameObject.CompareTag("Ability2_Fire")))
                    {
                        damage_a *= 2;
                    }

                    currentHealth -= damage;
                    currentHealth -= damage_a;

                    Debug.Log($"Dany aplicat: {damage} (Salut restant: {currentHealth})");
                    Debug.Log($"Dany aplicat: {damage_a} (Salut restant: {currentHealth})");

        
                }

                // Comprovació de mort
                if (currentHealth <= 0)
                {
                    die();
                    Debug.Log("Enemic derrotat!");
                }
            }
        }



        // Detecta col·lisions de partícules
        private void OnParticleCollision(GameObject other)
        {
            // Verifica si és una Ability2
            if (other.gameObject.CompareTag("Ability2_Ice") || other.gameObject.CompareTag("Ability2_Lightning"))
            {
                
                PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
                damage_a = playerAttack.Ability_2_Damage;

                // Dany x2 si coincideix l'element
                if ((gameObject.CompareTag("Enemy_Ice") && other.gameObject.CompareTag("Ability2_Ice")) ||
                    (gameObject.CompareTag("Enemy_Lightning") && other.gameObject.CompareTag("Ability2_Lightning")) ||
                    (gameObject.CompareTag("Boss_Ice") && other.gameObject.CompareTag("Ability2_Ice")) ||
                    (gameObject.CompareTag("Boss_Lightning") && other.gameObject.CompareTag("Ability2_Lightning")))
                {
                    damage_a *= 2;
                }

                currentHealth -= damage_a;
                Debug.Log($"Dany de {other.tag}: {damage_a} (Salut restant: {currentHealth})");


                if (currentHealth <= 0)
                {
                    die();
                    Debug.Log("Enemic derrotat!");
                }

      

                

            }

        }

        public void die()
        {
            if (spawner != null)
            {
                spawner.EnemyDefeated(this.gameObject); // Notifica al Spawner
            }
            string username = PlayerPrefs.GetString("LoggedInUser", "Offline");
            string rawEnemyType = gameObject.tag; // Ex: "Enemy_Melee_Fire"

           
            string formattedEnemyType = rawEnemyType.Replace("Enemy_", "");
            

            
            if (formattedEnemyType.StartsWith("Melee_"))
            {
                formattedEnemyType = "Melee" + formattedEnemyType.Substring(6);
            }

            Debug.Log($"Enemy defeated: {formattedEnemyType}");

            PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
            string weaponType = (playerAttack != null) ? playerAttack.currentWeaponType : "";

            if (currentHealth <= 0 && damageMelee > 0)
            {
                weaponType = "Sword";
            }

            
            weaponType = weaponType.Replace("_", "");

            if (PlayerPrefs.GetInt("IsOnline") == 1 && MongoDBManager.Instance != null)
            {
                MongoDBManager.Instance.UpdateUserStats(username, formattedEnemyType, weaponType);
            }

            // Destruir l'enemic
            if (gameObject.CompareTag("Enemy_Fire") || gameObject.CompareTag("Enemy_Ice") || gameObject.CompareTag("Enemy_Lightning"))
            {
                Animator animator2 = GetComponent<Animator>();
                if (animator2 != null)
                {
                    animator2.Play("die");
                    Destroy(gameObject, 1f);
                }
            }
            else if (gameObject.CompareTag("Enemy_Melee_Fire") || gameObject.CompareTag("Enemy_Melee_Ice") || gameObject.CompareTag("Enemy_Melee_Lightning"))
            {
                Animator animator = GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play("Die");
                    Destroy(gameObject, 1f);
                }
            }

            else if (gameObject.CompareTag("Boss_Fire") || gameObject.CompareTag("Boss_Ice") || gameObject.CompareTag("Boss_Lightning"))
            {
                Animator animator3 = GetComponent<Animator>();
                if (animator3 != null)
                {
                    animator3.Play("DieBoss");
                    Destroy(gameObject, 1.5f);
                }
            }

            if (formattedEnemyType == "Fire")
                PlayerPrefs.SetInt("CurrentFireEnemies", PlayerPrefs.GetInt("CurrentFireEnemies") + 1);

            else if (formattedEnemyType == "Ice")
                PlayerPrefs.SetInt("CurrentIceEnemies", PlayerPrefs.GetInt("CurrentIceEnemies") + 1);

            else if (formattedEnemyType == "Lightning")
                PlayerPrefs.SetInt("CurrentLightningEnemies", PlayerPrefs.GetInt("CurrentLightningEnemies") + 1);
            
            else if (formattedEnemyType == "MeleeFire")
                PlayerPrefs.SetInt("CurrentMeleeFireEnemies", PlayerPrefs.GetInt("CurrentMeleeFireEnemies") + 1);

            else if (formattedEnemyType == "MeleeIce")
                PlayerPrefs.SetInt("CurrentMeleeIceEnemies", PlayerPrefs.GetInt("CurrentMeleeIceEnemies") + 1);

            else if (formattedEnemyType == "MeleeLightning")
                PlayerPrefs.SetInt("CurrentMeleeLightningEnemies", PlayerPrefs.GetInt("CurrentMeleeLightningEnemies") + 1);

            // Actualitza armes utilitzades al Game‑Over
            if (!string.IsNullOrEmpty(weaponType))
            {
                switch (weaponType)
                {
                    case "ProjectileFire":
                        PlayerPrefs.SetInt("CurrentProjectileFire",
                            PlayerPrefs.GetInt("CurrentProjectileFire") + 1);
                        break;
                    case "ProjectileIce":
                        PlayerPrefs.SetInt("CurrentProjectileIce",
                            PlayerPrefs.GetInt("CurrentProjectileIce") + 1);
                        break;
                    case "ProjectileLightning":
                        PlayerPrefs.SetInt("CurrentProjectileLightning",
                            PlayerPrefs.GetInt("CurrentProjectileLightning") + 1);
                        break;
                    case "Ability2Fire":
                        PlayerPrefs.SetInt("CurrentAbility2Fire",
                            PlayerPrefs.GetInt("CurrentAbility2Fire") + 1);
                        break;
                    case "Ability2Ice":
                        PlayerPrefs.SetInt("CurrentAbility2Ice",
                            PlayerPrefs.GetInt("CurrentAbility2Ice") + 1);
                        break;
                    case "Ability2Lightning":
                        PlayerPrefs.SetInt("CurrentAbility2Lightning",
                            PlayerPrefs.GetInt("CurrentAbility2Lightning") + 1);
                        break;
                    case "Sword":
                        PlayerPrefs.SetInt("CurrentSword",
                            PlayerPrefs.GetInt("CurrentSword") + 1);
                        break;
                }
            }
            PlayerPrefs.Save();


        }
    }
}