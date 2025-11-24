namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class MountainBiome : Biome
    {
        public override string Name => "Mountain Biome";
        public override float BaseFrequency => 0.008f;
        public override float BaseStrength => 12f;

        public override float MountainFrequency => 0.006f;
        public override float MountainStrength => 80f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 10f;

        public override int HeightOffset => 60;

        public override int MinPossibleHeight => 60;
        public override int MaxPossibleHeight => 110;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.6f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}