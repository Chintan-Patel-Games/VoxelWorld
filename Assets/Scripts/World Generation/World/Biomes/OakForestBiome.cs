using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public class OakForestBiome : Biome
    {
        public override string Name => "OakForest";

        public override float HeightMultiplier => 12f;
        public override float HeightOffset => 22f;

        public override float TreeChance => 0.03f;

        public override BlockType GetSurfaceBlock(int height)
            => BlockType.Grass;

        public override void GenerateTrees(
            Chunk chunk, int x, int y, int z,
            List<Vector3Int> treePositions)
        {
            // add the oak forest tree logic here
            treePositions.Add(new Vector3Int(x, y + 1, z));
        }
    }
}