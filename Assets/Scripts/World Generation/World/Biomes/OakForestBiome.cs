namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class OakForestBiome : Biome
    {
        public override string Name => "Oak Forest Biome";
        public override float BaseFrequency => 0.01f;
        public override float BaseStrength => 14f;

        public override float MountainFrequency => 0.003f;
        public override float MountainStrength => 8f;

        public override float VariationFrequency => 0.04f;
        public override float VariationStrength => 3f;

        public override int HeightOffset => 45;

        // Properties for tree generation
        public override bool CanGrowTrees => true;
        public override float TreeSpawnThreshold => 0.6f;
        public override int TreeMinHeight => 5;
        public override int TreeHeightVariation => 4;
    }
}