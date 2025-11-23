using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkModel
    {
        public Block[,,] blocks;

        public ChunkModel(int size, int height)
        {
            blocks = new Block[size, height, size];
        }
    }
}