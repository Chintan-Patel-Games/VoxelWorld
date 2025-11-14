using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.World.Biomes
{
    public abstract class Biome
    {
        // Name (for debugging/logging)
        public abstract string Name { get; }

        // Height controls
        public abstract float HeightMultiplier { get; }
        public abstract float HeightOffset { get; }

        // Tree controls
        public abstract float TreeChance { get; }

        // Gets the surface block type (grass, snow, etc.)
        public abstract BlockType GetSurfaceBlock(int height);

        // Biomes that generate trees override this
        public virtual void GenerateTrees(
            Chunk chunk,
            int x, int y, int z,
            List<Vector3Int> treePositions)
        {
            // Default: no trees
        }
    }
}