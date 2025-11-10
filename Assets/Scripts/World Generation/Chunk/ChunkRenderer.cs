using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk;
        public GameObject cubePrefab;

        void Start()
        {
            RenderChunk();
        }

        public void RenderChunk()
        {
            if (chunk == null)
            {
                chunk = GetComponent<Chunk>();
            }

            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int y = 0; y < Chunk.chunkHeight; y++)
                {
                    for (int z = 0; z < Chunk.chunkSize; z++)
                    {
                        Block block = chunk.blocks[x, y, z];

                        if (block.blockType != BlockType.Air)
                        {
                            Vector3 pos = new Vector3(block.position.x, block.position.y, block.position.z);
                            Instantiate(cubePrefab, pos + transform.position, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
}