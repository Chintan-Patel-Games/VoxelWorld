namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class MountainBiome : Biome
    {
        public override float BaseFrequency => 0.006f;
        public override float BaseStrength => 16f;

        public override float MountainFrequency => 0.001f;
        public override float MountainStrength => 18f;

        public override float VariationFrequency => 0.02f;
        public override float VariationStrength => 10f;

        public override float HeightOffset => 10;
        public override float HeightMultiplier => 1.5f;
        public override float MinHeight => 0f;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.6f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}