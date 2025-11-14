using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class MountainBiome : Biome
    {
        public override string Name => "Mountains";

        public override float HeightMultiplier => 60f;
        public override float HeightOffset => 30f;

        public override float TreeChance => 0.01f;

        public override BlockType GetSurfaceBlock(int height)
            => BlockType.Stone;
    }
}