using UnityEngine;

using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class Chunk : MonoBehaviour
    {
        public Chunk chunkNorth;
        public Chunk chunkSouth;
        public Chunk chunkEast;
        public Chunk chunkWest;

        public static int chunkSize = 16;  // width/depth
        public static int chunkHeight = 128; // vertical height (if you separate later)
        public Block[,,] blocks;
        public bool IsMeshReady { get; private set; } = false;

        void Awake() => blocks = new Block[chunkSize, chunkHeight, chunkSize];

        public void BuildMesh()
        {
            GetComponent<ChunkMeshGenerator>().GenerateMesh();
            IsMeshReady = true; // Mesh finished building
        }
    }
}