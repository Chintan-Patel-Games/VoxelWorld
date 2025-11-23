using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.World.Biomes;

namespace VoxelWorld.WorldGeneration.World
{
    public class TerrainService
    {
        private int seed;
        private BiomeProvider biomeProvider;

        // Constructor
        public TerrainService(int seed, BiomeProvider biomeProvider)
        {
            this.seed = seed;
            this.biomeProvider = biomeProvider;
        }

        // The main height function (final terrain logic)
        public int GetSurfaceHeight(int worldX, int worldZ)
        {
            // Fetch biome for this position
            Biome biome = biomeProvider.GetBiome(worldX, worldZ);

            // Base shape noise
            float n1 = Mathf.PerlinNoise(
                (worldX + seed) * biome.BaseFrequency,
                (worldZ + seed) * biome.BaseFrequency
            );

            // Mountain amplification noise
            float n2 = Mathf.PerlinNoise(
                (worldX + seed * 2) * biome.MountainFrequency,
                (worldZ + seed * 2) * biome.MountainFrequency
            );

            // Variation small noise
            float n3 = Mathf.PerlinNoise(
                (worldX + seed * 3) * biome.VariationFrequency,
                (worldZ + seed * 3) * biome.VariationFrequency
            );

            // Final height formula
            int height =
                Mathf.FloorToInt(n1 * biome.HeightMultiplier) +
                Mathf.FloorToInt(n2 * biome.MountainStrength) +
                Mathf.FloorToInt(n3 * biome.VariationStrength) +
                biome.HeightOffset;

            return Mathf.Clamp(height, 1, ChunkService.chunkHeight - 2);
        }

        // Returns block type at specific position
        public BlockType GetBlockTypeAt(int worldX, int worldZ, int y)
        {
            int surface = GetSurfaceHeight(worldX, worldZ);

            if (y > surface)
                return BlockType.Air;

            if (y == surface)
                return BlockType.Grass;

            if (y >= surface - 4)
                return BlockType.Dirt;

            return BlockType.Stone;
        }
    }
}