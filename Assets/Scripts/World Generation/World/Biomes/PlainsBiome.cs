namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class PlainsBiome : Biome
    {
        public override float BaseFrequency => 0.015f;
        public override float HeightMultiplier => 10f;

        public override float MountainFrequency => 0.001f;
        public override float MountainStrength => 0f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 2f;

        public override int HeightOffset => 20;
    }
}