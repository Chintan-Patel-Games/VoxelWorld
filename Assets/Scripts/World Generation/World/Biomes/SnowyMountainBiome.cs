using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class SnowyMountainBiome : Biome
    {
        public override string Name => "SnowyMountains";

        public override float HeightMultiplier => 70f;
        public override float HeightOffset => 35f;

        public override float TreeChance => 0f;

        public override BlockType GetSurfaceBlock(int height)
        {
            // snow above Y = 40
            if (height > 40)
                return BlockType.Snow;

            return BlockType.Stone;
        }
    }
}