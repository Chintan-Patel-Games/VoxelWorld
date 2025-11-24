namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class OakForestBiome : Biome
    {
        public override string Name => "Oak Forest Biome";
        public override float BaseFrequency => 0.001f;
        public override float BaseStrength => 10f;

        public override float MountainFrequency => 0.003f;
        public override float MountainStrength => 6f;

        public override float VariationFrequency => 0.02f;
        public override float VariationStrength => 2.5f;

        public override int HeightOffset => 45;

        public override int MinPossibleHeight => 45;
        public override int MaxPossibleHeight => 70;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.58f;
        public override int TreeMinHeight => 5;
        public override int TreeHeightVariation => 3;
    }
}