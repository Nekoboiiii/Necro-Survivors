using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private PlayerMovement pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks = new List<GameObject>();
    public float maxOpDist;
    public float optimizerCooldownDuration;
    
    private float optimizerCooldown;
    private readonly Dictionary<string, Vector3> directionOffsets = new Dictionary<string, Vector3>();

    void Start()
    {
        pm = player.GetComponent<PlayerMovement>();

        // Store relative positions to check for new chunks
        directionOffsets.Add("Right", Vector3.right);
        directionOffsets.Add("Left", Vector3.left);
        directionOffsets.Add("Up", Vector3.up);
        directionOffsets.Add("Down", Vector3.down);
        directionOffsets.Add("Right Up", new Vector3(1, 1, 0));
        directionOffsets.Add("Left Up", new Vector3(-1, 1, 0));
        directionOffsets.Add("Right Down", new Vector3(1, -1, 0));
        directionOffsets.Add("Left Down", new Vector3(-1, -1, 0));
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk) return;

        foreach (var direction in directionOffsets)
        {
            Transform checkPoint = currentChunk.transform.Find(direction.Key);
            if (checkPoint && !Physics2D.OverlapCircle(checkPoint.position, checkerRadius, terrainMask))
            {
                SpawnChunk(checkPoint.position);
            }
        }
    }

    void SpawnChunk(Vector3 position)
    {
        GameObject newChunk = Instantiate(terrainChunks[Random.Range(0, terrainChunks.Count)], position, Quaternion.identity);
        spawnedChunks.Add(newChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0) return;
        optimizerCooldown = optimizerCooldownDuration;

        Vector3 playerPos = player.transform.position;
        foreach (GameObject chunk in spawnedChunks)
        {
            chunk.SetActive(Vector3.Distance(chunk.transform.position, playerPos) <= maxOpDist);
        }
    }
}
