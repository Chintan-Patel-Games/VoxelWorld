using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration
{
    public class WorldGenerator : MonoBehaviour
    {
        public GameObject chunkPrefab;
        public int worldSizeInChunks = 32;  // for testing, 2x2 chunks
        public float noiseScale = 0.065f;   // controls terrain smoothness
        public int heightMultiplier = 5;  // max terrain height

        void Start() => GenerateWorld();

        void GenerateWorld()
        {
            int halfSize = worldSizeInChunks / 2;

            for (int x = -halfSize; x < halfSize; x++)
            {
                for (int z = -halfSize; z < halfSize; z++)
                {
                    CreateChunk(x, z);
                }
            }
        }

        void CreateChunk(int chunkX, int chunkZ)
        {
            Vector3 pos = new Vector3(chunkX * Chunk.chunkSize, 0, chunkZ * Chunk.chunkSize);
            GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
            Chunk chunk = chunkObj.GetComponent<Chunk>();
            FillChunkData(chunk, chunkX, chunkZ);
            chunk.BuildMesh();
        }

        void FillChunkData(Chunk chunk, int chunkX, int chunkZ)
        {
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int z = 0; z < Chunk.chunkSize; z++)
                {
                    int worldX = x + chunkX * Chunk.chunkSize;
                    int worldZ = z + chunkZ * Chunk.chunkSize;

                    // smoother and controlled plains
                    float noise = Mathf.PerlinNoise((worldX + 1000) * noiseScale, (worldZ + 1000) * noiseScale);
                    int surfaceHeight = Mathf.RoundToInt(noise * heightMultiplier) + 10; // keep low enough to fit in chunk

                    for (int y = 0; y < Chunk.chunkSize; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);

                        if (y > surfaceHeight)
                            chunk.blocks[x, y, z] = new Block(BlockType.Air, pos);
                        else if (y == surfaceHeight)
                            chunk.blocks[x, y, z] = new Block(BlockType.Grass, pos);  // always grass on the very top
                        else if (y < surfaceHeight && y >= surfaceHeight - 5)
                            chunk.blocks[x, y, z] = new Block(BlockType.Dirt, pos);  // next 5 blocks = dirt
                        else
                            chunk.blocks[x, y, z] = new Block(BlockType.Stone, pos);  // everything below = stone
                    }
                }
            }
        }
    }
}