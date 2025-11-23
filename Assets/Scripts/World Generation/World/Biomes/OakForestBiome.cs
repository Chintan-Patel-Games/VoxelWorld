namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class OakForestBiome : Biome
    {
        public override float BaseFrequency => 0.01f;
        public override float HeightMultiplier => 10f;

        public override float MountainFrequency => 0.003f;
        public override float MountainStrength => 4f;

        public override float VariationFrequency => 0.04f;
        public override float VariationStrength => 3f;

        public override int HeightOffset => 22;
    }
}