using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class DarkOakForestBiome : Biome
    {
        public override string Name => "DarkOakForest";

        public override float HeightMultiplier => 12f;
        public override float HeightOffset => 22f;

        public override float TreeChance => 0.02f;

        public override BlockType GetSurfaceBlock(int height)
            => BlockType.Grass;

        public override void GenerateTrees(
            Chunk chunk, int x, int y, int z,
            List<Vector3Int> treePositions)
        {
            // later: big dark oak trees
            treePositions.Add(new Vector3Int(x, y + 1, z));
        }
    }
}