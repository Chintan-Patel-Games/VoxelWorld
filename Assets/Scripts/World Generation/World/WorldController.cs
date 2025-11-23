using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World
{
    public class WorldController : GenericMonoSingleton<WorldController>
    {
        [Header("World")]
        public GameObject chunkPrefab;
        public int seed = 12345;
        public float chunkLoadDelay = 0f;
        public int VIEW_RADIUS = 4;  // visible mesh radius
        public int LOAD_RADIUS = 6;  // chunk load radius
        public int UNLOAD_DATA_RADIUS = 10; // safe radius before destroying chunk data
        public int simulationDistance = 1; // chunks radius where colliders & simulation are enabled

        [Header("Player")]
        public Transform player;

        private WorldService worldService;
        private ChunkService chunkService;

        // Keep track of which coords currently have meshes built (visible)
        private readonly HashSet<Vector2Int> visibleSet = new HashSet<Vector2Int>();

        // Keep track of which coords are loaded (have chunk data)
        private readonly HashSet<Vector2Int> loadedSet = new HashSet<Vector2Int>();

        public void Init(WorldService worldService)
        {
            this.worldService = worldService;
            chunkService = worldService.GetChunkService();
        }

        void Update()
        {
            if (player == null || worldService == null) return;
            StreamAroundPlayer();
        }

        private void StreamAroundPlayer()
        {
            Vector2Int playerCoord = worldService.WorldToChunkCoord(player.position);

            // Build needed sets
            HashSet<Vector2Int> neededLoad = new HashSet<Vector2Int>();
            HashSet<Vector2Int> neededView = new HashSet<Vector2Int>();

            // LOAD CHUNK RING
            for (int dx = -LOAD_RADIUS; dx <= LOAD_RADIUS; dx++)
                for (int dz = -LOAD_RADIUS; dz <= LOAD_RADIUS; dz++)
                    neededLoad.Add(new Vector2Int(playerCoord.x + dx, playerCoord.y + dz));

            // VIEW CHUNK RING
            for (int dx = -VIEW_RADIUS; dx <= VIEW_RADIUS; dx++)
                for (int dz = -VIEW_RADIUS; dz <= VIEW_RADIUS; dz++)
                    neededView.Add(new Vector2Int(playerCoord.x + dx, playerCoord.y + dz));

            foreach (var coord in neededLoad)
            {
                if (!chunkService.HasChunk(coord) && !IsGenerating(coord))
                {
                    // Chunk data generation; onChunkReady lets us set initial simulation flag
                    chunkService.GenerateChunk(
                        coord,
                        0f,
                        (cx, y, cz) =>
                        {
                            int worldX = coord.x * ChunkService.chunkSize + cx;
                            int worldZ = coord.y * ChunkService.chunkSize + cz;
                            return worldService.GetTerrainService().GetBlockTypeAt(worldX, worldZ, y);
                        },
                        onChunkReady: (chunk) =>
                        {
                            // set RequiresCollider if inside simulationDistance of player
                            int dx = Mathf.Abs(chunk.Coord.x - playerCoord.x);
                            int dz = Mathf.Abs(chunk.Coord.y - playerCoord.y);
                            chunk.RequiresCollider = (dx <= simulationDistance && dz <= simulationDistance);
                        }
                    );
                }
            }

            // Unload (destroy) chunks that are outside LOAD_RADIUS
            // Instead of removing them from ActiveChunks immediately, ensure they are not in neededLoad.
            List<Vector2Int> toUnloadMesh = new List<Vector2Int>();

            foreach (var coord in visibleSet)
                if (!neededLoad.Contains(coord))
                    toUnloadMesh.Add(coord);

            foreach (var coord in toUnloadMesh)
            {
                chunkService.UnloadChunkMesh(coord);
                visibleSet.Remove(coord);
            }

            // DESTROY CHUNK DATA ONLY IF VERY FAR
            List<Vector2Int> toDestroyData = new();

            foreach (var kv in chunkService.ActiveChunks)
            {
                Vector2Int coord = kv.Key;
                int dist = Mathf.Max(
                    Mathf.Abs(coord.x - playerCoord.x),
                    Mathf.Abs(coord.y - playerCoord.y)
                );

                if (dist > UNLOAD_DATA_RADIUS)
                    toDestroyData.Add(coord);
            }

            foreach (var coord in toDestroyData)
            {
                chunkService.DestroyChunk(coord);
                visibleSet.Remove(coord);
            }

            // BUILD MESHES FOR ANY CHUNK INSIDE VIEW RADIUS
            foreach (var coord in neededView)
            {
                if (!chunkService.HasChunk(coord))
                    continue; // not loaded yet

                var chunk = chunkService.GetChunk(coord);
                if (!chunk.HasMesh)
                {
                    chunkService.BuildChunkMesh(coord);
                    visibleSet.Add(coord);
                }
            }

            // Update simulation flags (colliders & AI) around player
            UpdateSimulationFlags(playerCoord);
        }

        // Helper to check if a chunk is currently being generated (so we don't request twice)
        private bool IsGenerating(Vector2Int coord) => chunkService.IsGenerating(coord);

        private void UpdateSimulationFlags(Vector2Int playerCoord)
        {
            foreach (var kv in chunkService.ActiveChunks)
            {
                ChunkController chunk = kv.Value;

                int dx = Mathf.Abs(chunk.Coord.x - playerCoord.x);
                int dz = Mathf.Abs(chunk.Coord.y - playerCoord.y);

                bool shouldSim = (dx <= simulationDistance && dz <= simulationDistance);

                if (chunk.RequiresCollider != shouldSim)
                {
                    chunk.RequiresCollider = shouldSim;

                    if (shouldSim && chunk.HasMesh)
                    {
                        ChunkRunner.EnqueueColliderApply(chunk, chunk.View.meshFilter.sharedMesh);
                    }
                    else if (!shouldSim && chunk.View.meshCollider != null)
                    {
                        chunk.View.meshCollider.sharedMesh = null;
                    }
                }
            }
        }
    }
}