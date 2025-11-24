namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class DarkOakForestBiome : Biome
    {
        public override string Name => "Dark Oak Forest Biome";
        public override float BaseFrequency => 0.01f;
        public override float BaseStrength => 15f;

        public override float MountainFrequency => 0.003f;
        public override float MountainStrength => 5f;

        public override float VariationFrequency => 0.06f;
        public override float VariationStrength => 3f;

        public override int HeightOffset => 45;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.55f;
        public override int TreeMinHeight => 6;
        public override int TreeHeightVariation => 4;
    }
}