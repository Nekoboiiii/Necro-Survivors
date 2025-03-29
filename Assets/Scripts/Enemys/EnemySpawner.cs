using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // List of groups of enemies to spawn in this wave
        public int waveQuota; // Total Number of enemies in this Wave
        public float spawnInterval; // The interval at which to spawn enemies
        public int spawnCount; // The number of eneiems spawned in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // Number of enemies to spawn in this wave
        public int SpawnCount; // The number of enemies spawned in this wave
        public GameObject enemyPrefab;
    }
    
    public List<Wave> waves; // List of all the Waves in the Game
    public int currentWaveCount; // Index of the current wave

    [Header("Spawner Attributes")]
    float spawnTimer; // Timer used to determine when to spawn the next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed; // Maximum of number of enemies allowed on the map at once
    public bool maxEnemiesReached = false; // Indicates if the maximum number of enemies has been reached
    public float waveInterval; // Interval between each wave

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; // List to store all the relative spawn points

    Transform player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    
    }

    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0) // Check if the wave has ended and the next wave should start
        {
            StartCoroutine(BeginNextWave());
        }
        
        spawnTimer += Time.deltaTime;

        // Check if its time to spawn the nexz enemy
        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        // Wave for "waveInterval" seconds before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        // If there are more waves to start after the current wave, move on to the next wave
        if(currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota  = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    /// <summary>
    /// This method will stop spawning enemies if the amount of enemies on the map is maximum
    /// The method will only spawn enemies in a particular wave until it is time for the next waves enemies to be spawned
    /// </summary>
    void SpawnEnemies()
    {
        // Check if the minimum number of enemies in the wave have been spawned
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn each type of enemy until the quota is filled
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check if the minimum number of enemies have been spawned
                if(enemyGroup.SpawnCount < enemyGroup.enemyCount)
                {
                    //Limits the number of enemies that can be spawned at once
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    //Spawn the enemy at the a random position close to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.SpawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        // Reset the maxEnemies flag if the number of enemies alive has dropped below the maximum below the maximum amount
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
