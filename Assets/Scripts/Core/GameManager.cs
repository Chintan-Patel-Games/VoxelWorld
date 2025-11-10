using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject chunkPrefab;

        [Header("Generation Settings")]
        [SerializeField] private int renderDistance = 4;
        [SerializeField] private float loadDelay = 0.02f; // seconds between block/row loads

        private ChunkLoader chunkLoader;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            if (player == null || chunkPrefab == null)
            {
                Debug.LogError("GameManager: Missing Player or Chunk Prefab reference!");
                return;
            }

            // Initialize world systems
            GameObject loaderObj = new("ChunkLoader");
            chunkLoader = loaderObj.AddComponent<ChunkLoader>();

            chunkLoader.player = player;
            chunkLoader.chunkPrefab = chunkPrefab;
            chunkLoader.renderDistance = renderDistance;

            // Start world generation gradually
            StartCoroutine(LoadWorldGradually());
        }

        private IEnumerator LoadWorldGradually()
        {
            yield return new WaitForSeconds(0.5f); // small startup delay

            Vector2Int playerChunk = GetChunkCoordFromPosition(player.position);
            HashSet<Vector2Int> loaded = new();

            // 1. Generate player's own chunk first
            chunkLoader.CreateChunkGradually(playerChunk, loadDelay);
            loaded.Add(playerChunk);

            // Wait until player’s ground chunk mesh exists
            yield return new WaitUntil(() => chunkLoader.HasChunkAt(playerChunk));
            yield return new WaitForSeconds(0.5f); // short delay before expanding

            // 2. Generate surrounding chunks in concentric rings
            for (int radius = 1; radius <= renderDistance; radius++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        // Only outer ring positions
                        if (Mathf.Abs(x) != radius && Mathf.Abs(z) != radius)
                            continue;

                        Vector2Int coord = playerChunk + new Vector2Int(x, z);
                        if (!loaded.Contains(coord))
                        {
                            chunkLoader.CreateChunkGradually(coord, loadDelay);
                            loaded.Add(coord);
                            yield return new WaitForSeconds(loadDelay);
                        }
                    }
                }
            }

            Debug.Log("Initial world fully generated from player position!");
        }

        private Vector2Int GetChunkCoordFromPosition(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
            int z = Mathf.FloorToInt(pos.z / Chunk.chunkSize);
            return new Vector2Int(x, z);
        }
    }
}
