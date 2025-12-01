namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class PlainsBiome : Biome
    {
        public override float BaseFrequency => 0.004f;
        public override float BaseStrength => 8f;

        public override float MountainFrequency => 0.008f;
        public override float MountainStrength => 2f;

        public override float VariationFrequency => 0.012f;
        public override float VariationStrength => 4f;

        public override float HeightOffset => 2;
        public override float HeightMultiplier => 0.8f;
        public override float MinHeight => 0f;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.8f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}