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
        private readonly Dictionary<Vector2Int, Coroutine> generatingChunks = new();
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
            UpdateVisibleChunks(); // generate initial world
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

            // Determine visible chunk coordinates
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
                // Stop only the coroutine generating this chunk
                if (generatingChunks.TryGetValue(coord, out Coroutine routine))
                {
                    StopCoroutine(routine);
                    generatingChunks.Remove(coord);
                }

                // Destroy chunk safely
                if (activeChunks.TryGetValue(coord, out Chunk c))
                {
                    if (c != null && c.gameObject != null)
                        Destroy(c.gameObject);
                }

                activeChunks.Remove(coord);
            }
        }

        public void CreateChunkGradually(Vector2Int coord, float delay)
        {
            if (activeChunks.ContainsKey(coord) || generatingChunks.ContainsKey(coord))
                return;

            Coroutine routine = StartCoroutine(GenerateChunkStepByStep(coord, delay));
            generatingChunks[coord] = routine;
        }

        private IEnumerator GenerateChunkStepByStep(Vector2Int coord, float delay)
        {
            if (activeChunks.ContainsKey(coord))
                yield break;

            // Create chunk object
            Vector3 pos = new(coord.x * Chunk.chunkSize, 0, coord.y * Chunk.chunkSize);
            GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
            Chunk chunk = chunkObj.GetComponent<Chunk>();

            activeChunks[coord] = chunk;

            List<Vector3Int> treePositions = new(); // store tree bases

            // -------- Generate base terrain --------
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int z = 0; z < Chunk.chunkSize; z++)
                {
                    int worldX = x + coord.x * Chunk.chunkSize;
                    int worldZ = z + coord.y * Chunk.chunkSize;

                    float frequency = 0.015f;
                    float amplitude = 40f;
                    float baseHeight = 20f;

                    float noise = Mathf.PerlinNoise((worldX + 1000) * frequency, (worldZ + 1000) * frequency);
                    int surface = Mathf.RoundToInt(noise * amplitude + baseHeight);

                    for (int y = 0; y < Chunk.chunkHeight; y++)
                    {
                        Vector3Int posInChunk = new(x, y, z);

                        if (y > surface)
                            chunk.blocks[x, y, z] = new(BlockType.Air, posInChunk);
                        else if (y == surface)
                        {
                            chunk.blocks[x, y, z] = new(BlockType.Grass, posInChunk);

                            // Decide whether to spawn a tree here
                            if (Random.value < 0.05f && x > 2 && z > 2 && x < Chunk.chunkSize - 3 && z < Chunk.chunkSize - 3)
                                treePositions.Add(new Vector3Int(x, y + 1, z)); // store tree base
                        }
                        else if (y >= surface - 5)
                            chunk.blocks[x, y, z] = new(BlockType.Dirt, posInChunk);
                        else
                            chunk.blocks[x, y, z] = new(BlockType.Stone, posInChunk);
                    }
                }

                yield return new WaitForSeconds(delay);
            }

            // -------- Generate trees after terrain --------
            foreach (var basePos in treePositions)
                GenerateTree(chunk, basePos.x, basePos.y, basePos.z);

            // -------- Build Mesh (safe) --------
            if (chunk == null || chunk.gameObject == null)
                yield break;

            chunk.BuildMesh();

            // -------- Rebuild neighbors safely --------
            if (chunk.chunkNorth != null && chunk.chunkNorth.gameObject != null)
                chunk.chunkNorth.BuildMesh();
            if (chunk.chunkSouth != null && chunk.chunkSouth.gameObject != null)
                chunk.chunkSouth.BuildMesh();
            if (chunk.chunkEast != null && chunk.chunkEast.gameObject != null)
                chunk.chunkEast.BuildMesh();
            if (chunk.chunkWest != null && chunk.chunkWest.gameObject != null)
                chunk.chunkWest.BuildMesh();

            generatingChunks.Remove(coord);
        }

        private void GenerateTree(Chunk chunk, int x, int y, int z)
        {
            int trunkHeight = Random.Range(4, 6);
            int topY = y + trunkHeight;

            // --- Trunk ---
            for (int i = 0; i < trunkHeight; i++)
            {
                int trunkY = y + i;
                if (trunkY < Chunk.chunkHeight)
                    chunk.blocks[x, trunkY, z] = new Block(BlockType.Wood, new Vector3Int(x, trunkY, z));
            }

            // --- Leaves: bottom wide layer (5x5) ---
            int leafY = topY;
            for (int lx = -2; lx <= 2; lx++)
            {
                for (int lz = -2; lz <= 2; lz++)
                {
                    if (Mathf.Abs(lx) + Mathf.Abs(lz) > 3) continue;

                    int wx = x + lx;
                    int wz = z + lz;

                    if (wx >= 0 && wx < Chunk.chunkSize && wz >= 0 && wz < Chunk.chunkSize)
                        chunk.blocks[wx, leafY, wz] = new Block(BlockType.Leaves, new Vector3Int(wx, leafY, wz));
                }
            }

            // --- Top smaller 3x3 layer ---
            leafY++;
            for (int lx = -1; lx <= 1; lx++)
            {
                for (int lz = -1; lz <= 1; lz++)
                {
                    int wx = x + lx;
                    int wz = z + lz;

                    if (wx >= 0 && wx < Chunk.chunkSize && wz >= 0 && wz < Chunk.chunkSize)
                        chunk.blocks[wx, leafY, wz] = new Block(BlockType.Leaves, new Vector3Int(wx, leafY, wz));
                }
            }
        }

        public bool HasChunkAt(Vector2Int coord) => activeChunks.ContainsKey(coord);
    }
}