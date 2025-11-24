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

        public TerrainService(int seed, BiomeProvider biomeProvider)
        {
            this.seed = seed;
            this.biomeProvider = biomeProvider;
        }

        // The main height function (final terrain logic)
        public int GetSurfaceHeight(int worldX, int worldZ)
        {
            biomeProvider.GetBiomeWeights(worldX, worldZ, out var weights);

            float height = 0f;

            foreach (var w in weights)
            {
                float h = CalculateBiomeHeight(worldX, worldZ, w.biome);
                height += h * w.weight;
            }

            return Mathf.Clamp(Mathf.RoundToInt(height), 1, ChunkService.chunkHeight - 2);
        }

        // BIOME-SPECIFIC TERRAIN CALCULATION
        private float CalculateBiomeHeight(int x, int z, Biome biome)
        {
            // Base terrain (broad features)
            float baseNoise =
                Perlin2D(x, z, biome.BaseFrequency, seed) * biome.BaseStrength;

            // Ridge noise (for hills & mountains)
            float ridgeNoise =
                Mathf.Abs(Perlin2D(x, z, biome.MountainFrequency, seed * 2) - 0.5f) * 2f;
            ridgeNoise *= biome.MountainStrength;

            // Detail noise (minor bumps)
            float detailNoise =
                Perlin2D(x, z, biome.VariationFrequency, seed * 3) * biome.VariationStrength;

            return baseNoise + ridgeNoise + detailNoise + biome.HeightOffset;
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

        // Perlin Helper
        private float Perlin2D(int x, int z, float frequency, int seedOffset)
        {
            return Mathf.PerlinNoise(
                (x + seedOffset) * frequency,
                (z + seedOffset) * frequency
            );
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

            // Fallback: guarantees NEVER null
            if (biome == null)
            {
                biome = biomeProvider.DefaultBiome;
                Debug.LogWarning($"[TerrainService] Using fallback biome at {worldX},{worldZ}");
            }
        }
    }
}