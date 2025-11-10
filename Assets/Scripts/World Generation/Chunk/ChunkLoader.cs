using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkLoader : MonoBehaviour
    {
        [Header("References")]
        public Transform player;
        public GameObject chunkPrefab;

        [Header("Chunk Settings")]
        public int renderDistance = 4; // in chunks

        private readonly Dictionary<Vector2Int, Chunk> activeChunks = new();
        private Vector2Int currentPlayerChunk;

        void Start()
        {
            if (player == null)
            {
                Debug.LogError("ChunkLoader: Player reference not set!");
                enabled = false;
                return;
            }

            currentPlayerChunk = GetChunkCoordFromPosition(player.position);
        }

        void Update()
        {
            Vector2Int newPlayerChunk = GetChunkCoordFromPosition(player.position);

            if (newPlayerChunk != currentPlayerChunk)
            {
                currentPlayerChunk = newPlayerChunk;
                UpdateVisibleChunks();
            }
        }

        Vector2Int GetChunkCoordFromPosition(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / Chunk.chunkSize);
            int z = Mathf.FloorToInt(position.z / Chunk.chunkSize);
            return new Vector2Int(x, z);
        }

        void UpdateVisibleChunks()
        {
            HashSet<Vector2Int> newVisible = new();

            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector2Int coord = currentPlayerChunk + new Vector2Int(x, z);
                    newVisible.Add(coord);

                    if (!activeChunks.ContainsKey(coord))
                        CreateChunkGradually(coord, 0f);
                }
            }

            // Unload chunks outside range
            List<Vector2Int> toRemove = new();
            foreach (var coord in activeChunks.Keys)
            {
                if (!newVisible.Contains(coord))
                    toRemove.Add(coord);
            }

            foreach (var coord in toRemove)
            {
                Destroy(activeChunks[coord].gameObject);
                activeChunks.Remove(coord);
            }
        }

        public void CreateChunkGradually(Vector2Int coord, float delay)
        {
            if (!activeChunks.ContainsKey(coord))
                StartCoroutine(GenerateChunkStepByStep(coord, delay));
        }

        private IEnumerator GenerateChunkStepByStep(Vector2Int coord, float delay)
        {
            if (activeChunks.ContainsKey(coord))
                yield break;

            Vector3 pos = new(coord.x * Chunk.chunkSize, 0, coord.y * Chunk.chunkSize);
            GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);

            Chunk chunk = chunkObj.GetComponent<Chunk>();

            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int z = 0; z < Chunk.chunkSize; z++)
                {
                    int worldX = x + coord.x * Chunk.chunkSize;
                    int worldZ = z + coord.y * Chunk.chunkSize;

                    float noise = Mathf.PerlinNoise((worldX + 1000) * 0.065f, (worldZ + 1000) * 0.065f);
                    int surface = Mathf.RoundToInt(noise * 5f) + 10;

                    for (int y = 0; y < Chunk.chunkHeight; y++)
                    {
                        Vector3Int posInChunk = new(x, y, z);

                        if (y > surface)
                            chunk.blocks[x, y, z] = new(BlockType.Air, posInChunk);
                        else if (y == surface)
                            chunk.blocks[x, y, z] = new(BlockType.Grass, posInChunk);
                        else if (y >= surface - 5)
                            chunk.blocks[x, y, z] = new(BlockType.Dirt, posInChunk);
                        else
                            chunk.blocks[x, y, z] = new(BlockType.Stone, posInChunk);
                    }
                }

                yield return new WaitForSeconds(delay);
            }

            chunk.BuildMesh();
            activeChunks[coord] = chunk;
        }

        public bool HasChunkAt(Vector2Int coord) => activeChunks.ContainsKey(coord);
    }
}