using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;
 
    void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned on " + gameObject.name);
            return;
        }
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        PlayerStats foundPlayer = FindAnyObjectByType<PlayerStats>();
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogError("PlayerStats not found in the scene!");
        }
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(currentDamage);
            }
        }
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
    if (es != null)
    {
        es.OnEnemyKilled();
    }

    }

    void ReturnEnemy()
    {
    EnemySpawner es = FindAnyObjectByType<EnemySpawner>();

    if (es == null || es.relativeSpawnPoints == null || es.relativeSpawnPoints.Count == 0)
    {
        Debug.LogError("EnemySpawner or relativeSpawnPoints is missing!");
        return;
    }

    transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
}
}
