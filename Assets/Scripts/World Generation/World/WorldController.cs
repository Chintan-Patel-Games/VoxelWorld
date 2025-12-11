using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World
{
    public class WorldController : GenericMonoSingleton<WorldController>
    {
        [Header("World Settings")]
        public GameObject chunkPrefab;
        public int seed = 12345;
        public float chunkLoadDelay = 0f;
        public int VIEW_RADIUS = 8;  // visible mesh radius
        public int LOAD_RADIUS = 10;  // chunk load radius
        public int UNLOAD_DATA_RADIUS = 12; // safe radius before destroying chunk data
        public int simulationDistance = 2; // chunks radius where colliders & simulation are enabled

        [Header("Atmosphere Settings")]
        public Color fogColor = new(0.7f, 0.8f, 0.9f);
        public bool useLinearFog = true;
        public float fogDensity = 0.1f;

        [Header("Player")]
        public Transform player;

        private WorldService worldService;
        private ChunkService chunkService;

        // Keep track of which coords currently have meshes built (visible)
        private readonly HashSet<Vector2Int> visibleSet = new HashSet<Vector2Int>();
        private Vector2Int lastPlayerChunk = new Vector2Int(int.MinValue, int.MinValue);

        public void Init(WorldService worldService)
        {
            this.worldService = worldService;
            chunkService = worldService.GetChunkService();
        }

        void Update()
        {
            if (player == null || worldService == null) return;

            Vector2Int currentCoord = worldService.WorldToChunkCoord(player.position);

            if (currentCoord != lastPlayerChunk)
            {
                StreamAroundPlayer(currentCoord);
                lastPlayerChunk = currentCoord;
            }
        }

        private void StreamAroundPlayer(Vector2Int playerCoord)
        {
            HashSet<Vector2Int> neededLoad = new();
            foreach (var coord in GetChunkCoordsInRings(playerCoord, LOAD_RADIUS))
                neededLoad.Add(coord);

            HashSet<Vector2Int> neededView = new();
            foreach (var coord in GetChunkCoordsInRings(playerCoord, VIEW_RADIUS))
                neededView.Add(coord);

            foreach (var coord in neededLoad)
            {
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
                        int dx = Mathf.Abs(chunk.Coord.x - playerCoord.x);
                        int dz = Mathf.Abs(chunk.Coord.y - playerCoord.y);

                        chunk.RequiresCollider = (dx <= simulationDistance && dz <= simulationDistance);
                    }
                );
            }

            // Unload chunks that are outside LOAD_RADIUS
            List<Vector2Int> toUnloadMesh = new List<Vector2Int>();

            foreach (var coord in visibleSet)
                if (!neededView.Contains(coord))
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

                if (dist > UNLOAD_DATA_RADIUS && !neededLoad.Contains(coord))
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

        private IEnumerable<Vector2Int> GetChunkCoordsInRings(Vector2Int center, int radius)
        {
            // Ring 0 (player chunk)
            yield return center;

            // Rings 1..radius
            for (int r = 1; r <= radius; r++)
            {
                int xMin = center.x - r;
                int xMax = center.x + r;
                int zMin = center.y - r;
                int zMax = center.y + r;
                
                for (int x = xMin; x <= xMax; x++) yield return new Vector2Int(x, zMin);  // Bottom edge
                for (int x = xMin; x <= xMax; x++) yield return new Vector2Int(x, zMax);  // Top edge
                for (int z = zMin + 1; z < zMax; z++) yield return new Vector2Int(xMin, z);  // Left edge
                for (int z = zMin + 1; z < zMax; z++) yield return new Vector2Int(xMax, z);  // Right edge (excluding corners)
            }
        }

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

        public void SetupFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;

            if (useLinearFog)
            {
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = (VIEW_RADIUS - 1) * 16;
                RenderSettings.fogEndDistance = VIEW_RADIUS * 16 * 1.2f;
            }
            else
            {
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = fogDensity;
            }
        }
    }
}