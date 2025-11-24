namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class PlainsBiome : Biome
    {
        public override string Name => "Plains Biome";
        public override float BaseFrequency => 0.01f;
        public override float BaseStrength => 6f;

        public override float MountainFrequency => 0.003f;
        public override float MountainStrength => 2f;

        public override float VariationFrequency => 0.02f;
        public override float VariationStrength => 1f;

        public override int HeightOffset => 35;

        public override int MinPossibleHeight => 40;
        public override int MaxPossibleHeight => 60;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.88f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}