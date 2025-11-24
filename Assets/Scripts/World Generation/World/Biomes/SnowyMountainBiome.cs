namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class SnowyMountainBiome : Biome
    {
        public override string Name => "Snowy Mountain Biome";
        public override float BaseFrequency => 0.01f;
        public override float BaseStrength => 10f;

        public override float MountainFrequency => 0.01f;
        public override float MountainStrength => 55f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 12f;

        public override int HeightOffset => 70;

        // Properties for tree generation
        public override bool CanGrowTrees => false;
        public override float TreeSpawnThreshold => 0f;
        public override int TreeMinHeight => 0;
        public override int TreeHeightVariation => 0;
    }
}