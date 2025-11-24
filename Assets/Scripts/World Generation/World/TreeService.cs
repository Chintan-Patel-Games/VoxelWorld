using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.World.Biomes;

namespace VoxelWorld.WorldGeneration.World
{
    public class TreeService
    {
        private int seed;

        public TreeService(int seed) => this.seed = seed;

        // --------------- MAIN ENTRY ---------------
        public void TryPlaceTrees(ChunkController chunk, TerrainService terrain)
        {
            int size = ChunkService.chunkSize;

            for (int lx = 0; lx < size; lx++)
            {
                for (int lz = 0; lz < size; lz++)
                {
                    // convert to world coords
                    int worldX = chunk.Coord.x * size + lx;
                    int worldZ = chunk.Coord.y * size + lz;

                    // Must avoid overlapping with other trees
                    if (IsTreeNearby(chunk, worldX, worldZ, 4)) continue;

                    // get biome
                    Biome biome;
                    terrain.GetBiome(worldX, worldZ, out biome);

                    if (biome == null)
                    {
                        Debug.LogError($"[TreeService] biome is NULL at {worldX},{worldZ}");
                        continue;
                    }

                    // biome must allow trees
                    if (!biome.CanGrowTrees) continue;

                    // random chance
                    float noise = Mathf.PerlinNoise(
                        (worldX + seed) * 0.12f,
                        (worldZ + seed) * 0.12f
                    );

                    if (noise > biome.TreeSpawnThreshold)
                        PlaceTree(chunk, lx, lz, terrain, biome);
                }
            }
        }

        // --------------- TREE BUILDER ---------------
        private void PlaceTree(ChunkController chunk, int lx, int lz, TerrainService terrain, Biome biome)
        {
            if (biome == null) return;

            int size = ChunkService.chunkSize;

            // world coordinate for height
            int worldX = chunk.Coord.x * size + lx;
            int worldZ = chunk.Coord.y * size + lz;

            int surfaceY = terrain.GetSurfaceHeight(worldX, worldZ);

            // must place on grass
            Block baseBlock = chunk.Model.blocks[lx, surfaceY, lz];
            if (baseBlock == null || baseBlock.blockType != BlockType.Grass) return;

            // ensure tree stays inside the chunk (avoid cutting)
            if (lx < 2 || lx > size - 3 || lz < 2 || lz > size - 3)
                return;

            // generate tree
            int height = biome.TreeMinHeight + (int)(biome.TreeHeightVariation * biome.Noise(worldX, worldZ));

            BuildTrunk(chunk, lx, surfaceY + 1, lz, height);
            BuildLeaves(chunk, lx, surfaceY + height, lz);
        }

        private void BuildTrunk(ChunkController chunk, int x, int yStart, int z, int height)
        {
            for (int y = 0; y < height; y++)
            {
                int yy = yStart + y;
                if (yy >= ChunkService.chunkHeight) break;

                var blk = chunk.Model.blocks[x, yy, z];
                if (blk != null)
                    blk.blockType = BlockType.Wood;
            }
        }

        private void BuildLeaves(ChunkController chunk, int cx, int cy, int cz)
        {
            // ----- TOP LAYER (3x3) -----
            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++)
                {
                    int lx = cx + x;
                    int ly = cy;
                    int lz = cz + z;

                    if (!IsInsideChunk(lx, ly, lz)) continue;

                    var block = chunk.Model.blocks[lx, ly, lz];
                    if (block.blockType == BlockType.Air)
                        block.blockType = BlockType.Leaves;
                }

            // ----- MIDDLE LAYER (5x5) -----
            int layerY = cy - 1;

            for (int x = -2; x <= 2; x++)
                for (int z = -2; z <= 2; z++)
                {
                    int lx = cx + x;
                    int ly = layerY;
                    int lz = cz + z;

                    if (!IsInsideChunk(lx, ly, lz)) continue;

                    // OPTIONAL: Remove corners for more MC accuracy
                    if (Mathf.Abs(x) == 2 && Mathf.Abs(z) == 2)
                        continue;

                    var block = chunk.Model.blocks[lx, ly, lz];
                    if (block.blockType == BlockType.Air)
                        block.blockType = BlockType.Leaves;
                }
        }

        private bool IsInsideChunk(int x, int y, int z)
        {
            return x >= 0 && x < ChunkService.chunkSize &&
                   y >= 0 && y < ChunkService.chunkHeight &&
                   z >= 0 && z < ChunkService.chunkSize;
        }
        private bool IsTreeNearby(ChunkController chunk, int worldX, int worldZ, int minDist)
        {
            int size = ChunkService.chunkSize;

            // Convert world -> local chunk coords
            int cx = worldX - (chunk.Coord.x * size);
            int cz = worldZ - (chunk.Coord.y * size);

            for (int dx = -minDist; dx <= minDist; dx++)
            {
                for (int dz = -minDist; dz <= minDist; dz++)
                {
                    int lx = cx + dx;
                    int lz = cz + dz;

                    // Skip invalid local coords
                    if (lx < 0 || lx >= size || lz < 0 || lz >= size)
                        continue;

                    // Scan vertically for tree trunk
                    for (int y = 1; y < ChunkService.chunkHeight - 1; y++)
                    {
                        Block b = chunk.Model.blocks[lx, y, lz];
                        if (b != null && b.blockType == BlockType.Wood)
                        {
                            return true; // Tree found nearby
                        }
                    }
                }
            }

            return false;
        }
    }
}