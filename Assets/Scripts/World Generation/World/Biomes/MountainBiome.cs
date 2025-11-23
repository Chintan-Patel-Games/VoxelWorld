namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class MountainBiome : Biome
    {
        public override float BaseFrequency => 0.01f;
        public override float HeightMultiplier => 16f;

        public override float MountainFrequency => 0.008f;
        public override float MountainStrength => 40f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 4f;

        public override int HeightOffset => 28;
    }
}