namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class MountainBiome : Biome
    {
        public override string Name => "Mountain Biome";
        public override float BaseFrequency => 0.01f;
        public override float BaseStrength => 12f;

        public override float MountainFrequency => 0.008f;
        public override float MountainStrength => 45f;

        public override float VariationFrequency => 0.03f;
        public override float VariationStrength => 12f;

        public override int HeightOffset => 65;

        // Properties for tree generation
        public override bool CanGrowTrees => false;
        public override float TreeSpawnThreshold => 0f;
        public override int TreeMinHeight => 0;
        public override int TreeHeightVariation => 0;
    }
}