using UnityEngine;

using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class Chunk : MonoBehaviour
    {
        public static int chunkSize = 16;  // width/depth
        public static int chunkHeight = 64; // vertical height (if you separate later)
        public Block[,,] blocks;

        void Awake() => blocks = new Block[chunkSize, chunkHeight, chunkSize];

        public void BuildMesh() => GetComponent<ChunkMeshGenerator>().GenerateMesh();
    }
}