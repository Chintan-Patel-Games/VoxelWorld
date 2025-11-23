namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public abstract class Biome
    {
        public abstract float BaseFrequency { get; }
        public abstract float HeightMultiplier { get; }

        public abstract float MountainFrequency { get; }
        public abstract float MountainStrength { get; }

        public abstract float VariationFrequency { get; }
        public abstract float VariationStrength { get; }

        public abstract int HeightOffset { get; }
    }
}