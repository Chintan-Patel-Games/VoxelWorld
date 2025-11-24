using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class BiomeProvider
    {
        private readonly float biomeFrequency;
        private readonly int seed;

        private readonly List<BiomeThreshold> biomes = new();

        public Biome DefaultBiome => biomes[0].biome;

        public BiomeProvider(int seed, float biomeFrequency = 0.008f)
        {
            this.seed = seed;
            this.biomeFrequency = biomeFrequency;
        }

        public void AddBiome(Biome biome, float start, float end)
        {
            biomes.Add(new BiomeThreshold(biome, start, end));
        }

        public void GetBiomeWeights(int worldX, int worldZ, out List<(Biome biome, float weight)> weights)
        {
            weights = new List<(Biome, float)>(biomes.Count);

            float value = Mathf.PerlinNoise(
                (worldX + seed) * biomeFrequency,
                (worldZ + seed) * biomeFrequency
            );

            value = Mathf.Pow(value, 1.2f);   // stretches highs

            // smooth blend noise
            float blend = Mathf.PerlinNoise((worldX + seed * 5) * 0.005f, (worldZ + seed * 5) * 0.005f);

            for (int i = 0; i < biomes.Count; i++)
            {
                float start = biomes[i].start;
                float end = biomes[i].end;

                float t = Mathf.InverseLerp(start - 0.05f, end + 0.05f, value);

                // raise curve for smoother transition
                t = Mathf.SmoothStep(0, 1, t);

                // add subtle noise-based blend wobble
                t *= (0.85f + blend * 0.3f);

                weights.Add((biomes[i].biome, Mathf.Clamp01(t)));
            }

            // normalize
            float sum = 0;
            foreach (var w in weights) sum += w.weight;
            if (sum > 0)
            {
                for (int i = 0; i < weights.Count; i++)
                    weights[i] = (weights[i].biome, weights[i].weight / sum);
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