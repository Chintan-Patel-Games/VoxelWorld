using System.Collections;
using UnityEngine;
using Cinemachine;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.Core
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public ChunkLoader chunkLoader;
        public CinemachineVirtualCamera cinemachineCam;

        public Transform CurrentPlayer { get; private set; }

        public void SpawnPlayerAtPosition(Vector3 targetPos)
        {
            StartCoroutine(SpawnRoutine(targetPos));
        }

        private IEnumerator SpawnRoutine(Vector3 targetPos)
        {
            Vector2Int chunkCoord = new(
                Mathf.FloorToInt(targetPos.x / Chunk.chunkSize),
                Mathf.FloorToInt(targetPos.z / Chunk.chunkSize)
            );

            // Wait until that chunk exists
            while (!chunkLoader.HasChunkAt(chunkCoord))
                yield return null;

            // Find the ground surface
            int surfaceY = FindSurfaceY(targetPos, chunkCoord);

            Debug.Log($"SurfaceY found = {surfaceY} at chunk {chunkCoord}");
            Vector3 spawnPosition = new Vector3(targetPos.x + 0.5f, surfaceY + 2, targetPos.z + 0.5f);
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            CurrentPlayer = playerObj.transform;

            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = CurrentPlayer;
            }

            Debug.Log($"Player spawned safely at {spawnPosition}");
        }

        private int FindSurfaceY(Vector3 worldPos, Vector2Int chunkCoord)
        {
            Chunk chunk = chunkLoader.GetChunkAt(chunkCoord);

            if (chunk == null || chunk.blocks == null)
            {
                Debug.LogWarning($"Chunk missing or uninitialized at {chunkCoord}");
                return 20;
            }

            int localX = Mathf.FloorToInt(worldPos.x - chunkCoord.x * Chunk.chunkSize);
            int localZ = Mathf.FloorToInt(worldPos.z - chunkCoord.y * Chunk.chunkSize);

            // Clamp inside chunk bounds
            localX = Mathf.Clamp(localX, 0, Chunk.chunkSize - 1);
            localZ = Mathf.Clamp(localZ, 0, Chunk.chunkSize - 1);

            for (int y = Chunk.chunkHeight - 1; y >= 0; y--)
            {
                Block block = chunk.blocks[localX, y, localZ];
                if (block != null && block.blockType != BlockType.Air)
                    return y;
            }

            Debug.LogWarning($"No solid block found under player at {worldPos}");
            return 20; // fallback
        }
    }
}