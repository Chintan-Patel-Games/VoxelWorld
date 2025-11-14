using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class BiomeProvider
    {
        private readonly float biomeFrequency;
        private readonly int seed;

        private readonly List<BiomeThreshold> thresholds = new();

        public BiomeProvider(int seed, float biomeFrequency = 0.003f)
        {
            this.seed = seed;
            this.biomeFrequency = biomeFrequency;
        }

        public void AddBiome(Biome biome, float start, float end)
        {
            thresholds.Add(new BiomeThreshold(biome, start, end));
        }

        public Biome GetBiome(int worldX, int worldZ)
        {
            float value = Mathf.PerlinNoise(
                (worldX + seed) * biomeFrequency,
                (worldZ + seed) * biomeFrequency
            );

            foreach (var t in thresholds)
            {
                if (value >= t.start && value < t.end)
                    return t.biome;
            }

            return thresholds[0].biome; // fallback
        }

        private class BiomeThreshold
        {
            public Biome biome;
            public float start, end;

            public BiomeThreshold(Biome biome, float start, float end)
            {
                this.biome = biome;
                this.start = start;
                this.end = end;
            }
        }
    }
}