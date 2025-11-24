namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class SnowyMountainBiome : Biome
    {
        public override string Name => "Snowy Mountain Biome";
        public override float BaseFrequency => 0.008f;
        public override float BaseStrength => 12f;

        public override float MountainFrequency => 0.0065f;
        public override float MountainStrength => 95f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 12f;

        public override int HeightOffset => 70;

        public override int MinPossibleHeight => 70;
        public override int MaxPossibleHeight => 125;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.65f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}