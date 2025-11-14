using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class PlainsBiome : Biome
    {
        public override string Name => "Plains";

        public override float HeightMultiplier => 10f;
        public override float HeightOffset => 20f;

        public override float TreeChance => 0.01f; // almost none

        public override BlockType GetSurfaceBlock(int height)
            => BlockType.Grass;
    }
}