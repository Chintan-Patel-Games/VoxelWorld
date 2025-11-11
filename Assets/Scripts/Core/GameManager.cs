using Cinemachine;
using System.Collections;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private CinemachineVirtualCamera cinemachineCam;

        [Header("World Settings")]
        [SerializeField] private int renderDistance = 4;
        [SerializeField] private float loadDelay = 0.02f; // seconds between block/row loads

        private ChunkLoader chunkLoader;
        private Transform player;
        private Vector3 spawnPos;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            if (playerPrefab == null || chunkPrefab == null || cinemachineCam == null)
            {
                Debug.LogError("GameManager: Missing reference!");
                return;
            }

            // --- Step 1: Pick random world spawn position ---
            spawnPos = new Vector3(Random.Range(-200, 200), 0, Random.Range(-200, 200));
            Debug.Log($"Selected random spawn position: {spawnPos}");

            // --- Step 2: Setup ChunkLoader ---
            GameObject loaderObj = new("ChunkLoader");
            chunkLoader = loaderObj.AddComponent<ChunkLoader>();
            chunkLoader.chunkPrefab = chunkPrefab;
            chunkLoader.renderDistance = renderDistance;

            // --- Step 3: Start loading initial chunk ---
            StartCoroutine(GenerateInitialChunkAndSpawnPlayer());
        }

        private IEnumerator GenerateInitialChunkAndSpawnPlayer()
        {
            // Convert to chunk coordinates
            Vector2Int spawnChunk = GetChunkCoordFromPosition(spawnPos);
            Debug.Log($"Generating spawn chunk at: {spawnChunk}");

            // Generate that single chunk first
            chunkLoader.CreateChunkGradually(spawnChunk, loadDelay);

            // Wait until the chunk exists
            yield return new WaitUntil(() => chunkLoader.IsChunkReady(spawnChunk));
            yield return null; // wait one frame for collider update

            // --- Step 4: Calculate spawn height ---
            int surfaceY = FindSurfaceY(spawnPos, spawnChunk);
            Vector3 finalSpawn = new(spawnPos.x, surfaceY + 2f, spawnPos.z);
            Debug.Log($"Found surface at Y={surfaceY}, spawning player at {finalSpawn}");

            // --- Step 5: Spawn Player prefab ---
            GameObject playerObj = Instantiate(playerPrefab, finalSpawn, Quaternion.identity);
            player = playerObj.transform;
            chunkLoader.player = player; // Assign to loader for dynamic updates

            // --- Step 6: Attach Cinemachine ---
            // Safely find the PlayerCameraRoot child inside the prefab
            Transform playerFollowCamera = playerObj.transform.Find("PlayerCameraRoot");

            if (playerFollowCamera != null)
            {
                cinemachineCam.Follow = playerFollowCamera;
                cinemachineCam.LookAt = playerFollowCamera;
                Debug.Log("Cinemachine successfully linked to PlayerCameraRoot.");
            }
            else
            {
                Debug.LogWarning("PlayerCameraRoot not found in player prefab!");
            }

            Debug.Log("Cinemachine and player linked.");

            // --- Step 7: Gradually load surrounding chunks ---
            yield return StartCoroutine(GenerateSurroundingChunks(spawnChunk));
        }

        private IEnumerator GenerateSurroundingChunks(Vector2Int center)
        {
            for (int radius = 1; radius <= renderDistance; radius++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        if (Mathf.Abs(x) != radius && Mathf.Abs(z) != radius)
                            continue;

                        Vector2Int coord = center + new Vector2Int(x, z);

                        if (!chunkLoader.HasChunkAt(coord))
                        {
                            chunkLoader.CreateChunkGradually(coord, loadDelay);
                            yield return new WaitForSeconds(loadDelay);
                        }
                    }
                }
            }

            Debug.Log("World generation complete!");
        }

        private int FindSurfaceY(Vector3 worldPos, Vector2Int chunkCoord)
        {
            Chunk chunk = chunkLoader.GetChunkAt(chunkCoord);
            if (chunk == null) return 20;

            int localX = Mathf.FloorToInt(worldPos.x - chunkCoord.x * Chunk.chunkSize);
            int localZ = Mathf.FloorToInt(worldPos.z - chunkCoord.y * Chunk.chunkSize);

            for (int y = Chunk.chunkHeight - 1; y >= 0; y--)
            {
                Block block = chunk.blocks[localX, y, localZ];
                if (block != null && block.blockType != BlockType.Air)
                    return y;
            }

            return 20;
        }

        private Vector2Int GetChunkCoordFromPosition(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
            int z = Mathf.FloorToInt(pos.z / Chunk.chunkSize);
            return new Vector2Int(x, z);
        }
    }
}