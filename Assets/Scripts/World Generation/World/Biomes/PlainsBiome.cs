namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class PlainsBiome : Biome
    {
        public override string Name => "Plains Biome";
        public override float BaseFrequency => 0.015f;
        public override float BaseStrength => 10f;

        public override float MountainFrequency => 0.001f;
        public override float MountainStrength => 5f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 2f;

        public override int HeightOffset => 40;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.85f;
        public override int TreeMinHeight => 4;
        public override int TreeHeightVariation => 3;
    }
}