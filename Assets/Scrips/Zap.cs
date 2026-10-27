using Enemy;
using System.Linq;
using UnityEngine;

public class Zap : MonoBehaviour
{
    [SerializeField] private GameObject lightningEffectPrefab;
    [SerializeField] private float searchRadius = 30f;
    private int damage;

    private PlayerAttack playerAttack;
    private bool effectCreated = false;

    private void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        if (playerAttack != null)
        {
            damage = playerAttack.Ability_2_Damage;
        }

        GameObject enemy = FindClosestEnemy();

        if (enemy != null)
        {
            Vector3 zapPosition = enemy.transform.position + Vector3.up;
            transform.position = zapPosition;

            if (!effectCreated && lightningEffectPrefab != null)
            {
                Instantiate(lightningEffectPrefab, enemy.transform.position, Quaternion.identity);
                effectCreated = true;
            }

            Enemy_Vida enemyHealth = enemy.GetComponent<Enemy_Vida>();
            if (enemyHealth != null )
            {
                if (enemy.CompareTag("Enemy_Lightning") || enemy.CompareTag("Enemy_Melee_Lightning") || enemy.CompareTag("Boss_Lightning"))
                {
                    damage *= 2;
                }

                enemyHealth.currentHealth -= damage;
                Debug.Log($"Zap applied {damage} damage. Remaining health: {enemyHealth.currentHealth}");

                if (enemyHealth.currentHealth <= 0)
                {
                    enemyHealth.die();
                    
                }
            }

            Destroy(gameObject, 0.5f);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat cap enemic per al Zap!");
            Destroy(gameObject);
        }
    }

    private GameObject FindClosestEnemy()
    {
        string[] enemyTags = new string[]
        {
            "Enemy_Fire",
            "Enemy_Melee_Fire",
            "Enemy_Ice",
            "Enemy_Melee_Ice",
            "Enemy_Lightning",
            "Enemy_Melee_Lightning",
            "Boss_Fire",
            "Boss_Ice",
            "Boss_Lightning"


        };

        GameObject closestEnemy = null;
        float minDistance = searchRadius;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemiesWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject enemy in enemiesWithTag)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }


}
