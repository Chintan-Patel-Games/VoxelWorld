using UnityEngine;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public abstract class Biome
    {
        public abstract float BaseFrequency { get; }
        public abstract float BaseStrength { get; }

        public abstract float MountainFrequency { get; }
        public abstract float MountainStrength { get; }

        public abstract float VariationFrequency { get; }
        public abstract float VariationStrength { get; }

        public abstract float HeightOffset { get; }
        public abstract float HeightMultiplier { get; }
        public abstract float MinHeight { get; }

        public abstract bool CanGrowTrees { get; }
        public abstract float TreeSpawnThreshold { get; }
        public abstract int TreeMinHeight { get; }
        public abstract int TreeHeightVariation { get; }

        public virtual float Noise(int x, int z) => Mathf.PerlinNoise(x * 0.1f, z * 0.1f);
    }
}