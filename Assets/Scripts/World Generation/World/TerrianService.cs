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
        public BiomeProvider BiomeProvider => biomeProvider;

        public TerrainService(int seed, BiomeProvider biomeProvider)
        {
            this.seed = seed;
            this.biomeProvider = biomeProvider;
        }

        // The main height function (final terrain logic)
        public int GetSurfaceHeight(int worldX, int worldZ)
        {
            biomeProvider.GetBiomeWeights(worldX, worldZ, out var weights);

            float finalHeight = 0f;

            foreach (var w in weights)
            {
                float h = CalculateBiomeHeight(worldX, worldZ, w.biome);
                finalHeight += h * w.weight;
            }

            return Mathf.Clamp(Mathf.RoundToInt(finalHeight), 1, ChunkService.chunkHeight - 2);
        }

        // BIOME-SPECIFIC TERRAIN CALCULATION
        private float CalculateBiomeHeight(int x, int z, Biome biome)
        {
            float baseNoise = Perlin(x, z, biome.BaseFrequency, seed) * biome.BaseStrength;
            float variation = Perlin(x, z, biome.VariationFrequency, seed + 1) * biome.VariationStrength;

            // Soft mountain noise — no cliffs
            float mountain = Mathf.Abs(
                Perlin(x, z, biome.MountainFrequency, seed + 2) - 0.5f
            ) * 2f * biome.MountainStrength;

            return baseNoise + mountain + variation + biome.HeightOffset;
        }

        private float Perlin(int x, int z, float freq, int seedOff)
        {
            return Mathf.PerlinNoise(
                (x + seedOff) * freq,
                (z + seedOff) * freq
            );
        }

        // Returns block type at specific position
        public BlockType GetBlockTypeAt(int worldX, int worldZ, int y)
        {
            int surface = GetSurfaceHeight(worldX, worldZ);

            if (y > surface) return BlockType.Air;
            if (y == surface) return BlockType.Grass;
            if (y >= surface - 4) return BlockType.Dirt;

            return BlockType.Stone;
        }

        public void GetBiome(int worldX, int worldZ, out Biome biome)
        {
            biomeProvider.GetBiomeWeights(worldX, worldZ, out var weights);

            biome = null;
            float maxWeight = 0f;

            foreach (var w in weights)
            {
                if (w.weight > maxWeight)
                {
                    maxWeight = w.weight;
                    biome = w.biome;
                }
            }
        }
    }
}