using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class BiomeProvider
    {
        private readonly float biomeFrequency;
        private readonly int seed;
        private readonly List<BiomeThreshold> thresholds = new();
        private readonly float highBias;

        public BiomeProvider(int seed, float biomeFrequency = 0.008f, float highBias = 1.15f)
        {
            this.seed = seed;
            this.biomeFrequency = biomeFrequency;
            this.highBias = highBias;
        }

        public void AddBiome(Biome biome, float start, float end)
        {
            thresholds.Add(new BiomeThreshold(biome, start, end));
            thresholds.Sort((a, b) => a.start.CompareTo(b.start));
        }

        public Biome DefaultBiome => thresholds.Count > 0 ? thresholds[0].biome : null;

        public void GetBiomeWeights(int worldX, int worldZ, out List<BiomeWeight> weights)
        {
            weights = new List<BiomeWeight>(thresholds.Count);

            float n = Mathf.PerlinNoise((worldX + seed) * biomeFrequency, (worldZ + seed) * biomeFrequency);

            float total = 0f;

            foreach (var t in thresholds)
            {
                // weight based on distance from center of threshold
                float center = (t.start + t.end) * 0.5f;
                float w = Mathf.Exp(-Mathf.Pow((n - center) * 4f, 2));  // Gaussian blend
                weights.Add(new BiomeWeight(t.biome, w));
                total += w;
            }

            // normalize
            for (int i = 0; i < weights.Count; i++)
                weights[i] = new BiomeWeight(weights[i].biome, weights[i].weight / total);
        }

        public struct BiomeWeight
        {
            public Biome biome;
            public float weight;
            
            public BiomeWeight(Biome b, float w)
            {
                biome = b;
                weight = w;
            }
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