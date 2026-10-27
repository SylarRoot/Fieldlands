using Enemy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Afegir aquesta línia

public class Spawner : MonoBehaviour
{
    public GameObject[] objectToSpawn;
    public GameObject[]bossPrefab;

    public float initialSpawnInterval = 2f;
    public float spawnIntervalDecrease = 0.1f;
    public float minSpawnInterval = 0.5f;
    public Vector2 spawnRange = new Vector2(-10, 10);

    private int initialEnemyCount = 6;
    private int waveNumber = 1;
    private int currentEnemyCount;
    private List<GameObject> activeEnemies = new List<GameObject>(); // Llista d'enemics actius


    public TextMeshProUGUI WaveText;

    void Start()
    {
        InitializeWave();
    }

    void InitializeWave()
    {
        currentEnemyCount = initialEnemyCount + (waveNumber - 1) * 4;
        activeEnemies.Clear(); // Neteja la llista abans de nova onda
       
        if (waveNumber % 5 == 0)
        {
            currentEnemyCount = 1; // Només es spawneja 1 boss
        }
        else
        {
            currentEnemyCount = initialEnemyCount + (waveNumber - 1) * 4;
        }
        if (WaveText != null)
        {
            WaveText.gameObject.SetActive(true);
            WaveText.text = $"Wave: {waveNumber}";
        }

        for (int i = 0; i < currentEnemyCount; i++)
        {
            SpawnObject();

        }
    }

    void SpawnObject()
    {
        // Determinar si és ona de boss
        bool isBossWave = (waveNumber % 5 == 0);

        GameObject spawnedEnemy;
        if (isBossWave)
        {
            // Spawnejar el Boss en una posició central
            Vector3 spawnPosition = new Vector3(0, transform.position.y, transform.position.z);
            int randomIndex = Random.Range(0, bossPrefab.Length);
            spawnedEnemy = Instantiate(bossPrefab[randomIndex], spawnPosition, Quaternion.identity);
        }
        else
        {
            // Codi original per enemics normals
            float randomX = Random.Range(spawnRange.x, spawnRange.y);
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);
            int randomIndex = Random.Range(0, objectToSpawn.Length);
            spawnedEnemy = Instantiate(objectToSpawn[randomIndex], spawnPosition, Quaternion.identity);
        }

        activeEnemies.Add(spawnedEnemy);

        // Configurar salut (afegir escalat per al boss)
        Enemy_Vida enemyHealth = spawnedEnemy.GetComponent<Enemy_Vida>();
        if (enemyHealth != null)
        {
            enemyHealth.spawner = this;
            if (isBossWave)
            {
                // Augmentar salut del boss respecte a l'ona
                enemyHealth.maxHealth = 100 * waveNumber;
            }
            else
            {
                enemyHealth.maxHealth = Mathf.RoundToInt(enemyHealth.maxHealth * (1 + (waveNumber * 0.1f)));
            }
            enemyHealth.currentHealth = enemyHealth.maxHealth;
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        // Elimina l'enemic de la llista i comprova si queden enemics
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Enemics restants: {activeEnemies.Count}");
        }

        // Inicia la següent onda només si no queden enemics
        if (activeEnemies.Count == 0)
        {
            waveNumber++;
            InitializeWave();
        }
    }
}