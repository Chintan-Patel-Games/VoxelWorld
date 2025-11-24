using UnityEngine;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.World.Biomes;

namespace VoxelWorld.WorldGeneration.World
{
    public class WorldService
    {
        private ChunkService chunkService;
        private TerrainService terrainService;
        private BiomeProvider biomeProvider;

        private float loadDelay;

        public WorldService(GameObject chunkPrefab, int seed, float loadDelay)
        {
            this.loadDelay = loadDelay;

            // --- Setup Biomes ---
            biomeProvider = new BiomeProvider(seed, 0.005f, 1.25f);
            //biomeProvider.AddBiome(new OakForestBiome(), 0f, 0.35f);
            //biomeProvider.AddBiome(new DarkOakForestBiome(), 0.35f, 0.55f);
            //biomeProvider.AddBiome(new PlainsBiome(), 0.55f, 0.70f);
            //biomeProvider.AddBiome(new MountainBiome(), 0.70f, 0.85f);
            //biomeProvider.AddBiome(new SnowyMountainBiome(), 0.85f, 1.0f);
            biomeProvider.AddBiome(new OakForestBiome(), 0f, 0.5f);
            biomeProvider.AddBiome(new PlainsBiome(), 0.5f, 0.7f);
            biomeProvider.AddBiome(new MountainBiome(), 0.7f, 1.0f);

            // --- Terrain generator ---
            terrainService = new TerrainService(seed, biomeProvider);

            // --- Chunk handler ---
            chunkService = new ChunkService(chunkPrefab);
        }

        // Generate ONLY the player spawn chunk
        public void GenerateInitialChunk(Vector3 spawnPos)
        {
            Vector2Int coord = WorldToChunkCoord(spawnPos);

            chunkService.GenerateChunk(
                coord,
                loadDelay,
                (x, y, z) =>
                {
                    int worldX = coord.x * ChunkService.chunkSize + x;
                    int worldZ = coord.y * ChunkService.chunkSize + z;
                    return terrainService.GetBlockTypeAt(worldX, worldZ, y);
                },
                onChunkReady: null // we add vegetation later
            );
        }

        public bool IsChunkReady(Vector3 worldPos)
        {
            Vector2Int coord = WorldToChunkCoord(worldPos);
            ChunkController c = chunkService.GetChunk(coord);
            //return c != null && c.IsMeshReady;
            return c != null;
        }

        // Find height at world position
        public int GetSurfaceHeight(Vector3 worldPos)
        {
            int worldX = Mathf.FloorToInt(worldPos.x);
            int worldZ = Mathf.FloorToInt(worldPos.z);

            return terrainService.GetSurfaceHeight(worldX, worldZ);
        }

        // Helper
        public Vector2Int WorldToChunkCoord(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / ChunkService.chunkSize),
                Mathf.FloorToInt(pos.z / ChunkService.chunkSize)
            );
        }

        // For GameManager later
        public ChunkService GetChunkService() => chunkService;
        public TerrainService GetTerrainService() => terrainService;

        public void StartStreamingFromPlayer(Transform player, WorldController controller)
        {
            if (controller == null)
            {
                Debug.LogError("WorldController reference missing!");
                return;
            }

            controller.Init(this);
            controller.player = player;
        }
    }
}