namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class OakForestBiome : Biome
    {
        public override float BaseFrequency => 0.005f;
        public override float BaseStrength => 10f;

        public override float MountainFrequency => 0.001f;
        public override float MountainStrength => 4f;

        public override float VariationFrequency => 0.015f;
        public override float VariationStrength => 6f;

        public override float HeightOffset => 5;
        public override float HeightMultiplier => 1.2f;
        public override float MinHeight => 0f;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.5f;
        public override int TreeMinHeight => 5;
        public override int TreeHeightVariation => 3;
    }
}