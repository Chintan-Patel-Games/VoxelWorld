using UnityEngine;

namespace VoxelWorld.WorldGeneration.Blocks
{
    [System.Serializable]
    public class Block
    {
        public BlockType blockType;
        public Vector3Int position;

        public Block(BlockType type, Vector3Int pos)
        {
            blockType = type;
            position = pos;
        }
    }
}