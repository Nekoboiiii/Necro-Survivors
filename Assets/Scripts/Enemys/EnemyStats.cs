using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    
    //Current Stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feddback")]
    public Color damageColor = new Color(1, 0, 0, 1); //Color of the Flash
    public float damageFlashDuration =0.2f; // How long the flash should last
    public float deathFadeTime = 0.6f; // How much time it takres for the enemy to fade
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;
 
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

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage
        if(dmg > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        // Apply the knockback if its not zero.
        if (knockbackForce > 0)
        {
            // Gets the direction of the Knockback
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        //Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha =sr.color.a;

        // This is a loop that fires every frame
        while(t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            // Set the color for this frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

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
