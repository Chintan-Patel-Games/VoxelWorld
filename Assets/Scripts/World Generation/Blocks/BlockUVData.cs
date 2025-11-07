using UnityEngine;

namespace VoxelWorld.WorldGeneration.Blocks
{
    public static class BlockUVData
    {
        // Directions: 0=Front, 1=Back, 2=Left, 3=Right, 4=Top, 5=Bottom
        public static Vector2Int GetAtlasCoords(BlockType type, int dir)
        {
            switch (type)
            {
                case BlockType.Grass:
                    if (dir == 4) return new Vector2Int(0, 1); // Top face (grass)
                    if (dir == 5) return new Vector2Int(2, 4); // Bottom face (dirt)
                    return new Vector2Int(3, 4);               // Side faces

                case BlockType.Dirt:
                    return new Vector2Int(2, 4);

                case BlockType.Stone:
                    return new Vector2Int(2, 3);

                default:
                    return new Vector2Int(0, 3); // fallback tile
            }
        }
    }
}