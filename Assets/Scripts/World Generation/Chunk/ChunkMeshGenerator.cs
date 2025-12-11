using UnityEngine;
using System.Collections.Generic;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChunkMeshGenerator : MonoBehaviour
    {
        const float tileSize = 1f / 5f;
        private const float uvPadding = 0.001f;

        public Chunk chunk;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();
        private Mesh mesh;

        private Vector3[] faceDirections =
        {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };

        void Start() => chunk = GetComponent<Chunk>();

        public void GenerateMesh()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int y = 0; y < Chunk.chunkSize; y++)
                {
                    for (int z = 0; z < Chunk.chunkSize; z++)
                    {
                        Block block = chunk.blocks[x, y, z];

                        if (block.blockType == BlockType.Air) continue;

                        // Check each direction
                        for (int i = 0; i < 6; i++)
                        {
                            Vector3Int neighborPos = new Vector3Int(x, y, z) + Vector3Int.RoundToInt(faceDirections[i]);
                            if (!IsBlockSolid(neighborPos))
                                AddFace(i, new Vector3(x, y, z), block.blockType);
                        }
                    }
                }
            }

            mesh = new Mesh()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };

            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = mesh;

            var collider = GetComponent<MeshCollider>();
            if (collider != null)
            {
                collider.sharedMesh = null; // clear old reference
                collider.sharedMesh = mesh;
            }
        }

        bool IsBlockSolid(Vector3Int pos)
        {
            if (pos.x < 0 || pos.x >= Chunk.chunkSize ||
                pos.y < 0 || pos.y >= Chunk.chunkSize ||
                pos.z < 0 || pos.z >= Chunk.chunkSize)
                return false;

            return chunk.blocks[pos.x, pos.y, pos.z].blockType != BlockType.Air;
        }

        void AddFace(int dir, Vector3 pos, BlockType type)
        {
            int vCount = vertices.Count;

            Vector3[] v = new Vector3[4];
            switch (dir)
            {
                case 0: v = new[] { pos + new Vector3(0, 0, 1), pos + new Vector3(1, 0, 1), pos + new Vector3(1, 1, 1), pos + new Vector3(0, 1, 1) }; break; // Front
                case 1: v = new[] { pos + new Vector3(1, 0, 0), pos + new Vector3(0, 0, 0), pos + new Vector3(0, 1, 0), pos + new Vector3(1, 1, 0) }; break; // Back
                case 2: v = new[] { pos + new Vector3(0, 0, 0), pos + new Vector3(0, 0, 1), pos + new Vector3(0, 1, 1), pos + new Vector3(0, 1, 0) }; break; // Left
                case 3: v = new[] { pos + new Vector3(1, 0, 1), pos + new Vector3(1, 0, 0), pos + new Vector3(1, 1, 0), pos + new Vector3(1, 1, 1) }; break; // Right
                case 4: v = new[] { pos + new Vector3(0, 1, 1), pos + new Vector3(1, 1, 1), pos + new Vector3(1, 1, 0), pos + new Vector3(0, 1, 0) }; break; // Top
                case 5: v = new[] { pos + new Vector3(0, 0, 0), pos + new Vector3(1, 0, 0), pos + new Vector3(1, 0, 1), pos + new Vector3(0, 0, 1) }; break; // Bottom
            }

            vertices.AddRange(v);
            triangles.AddRange(new int[] { vCount, vCount + 1, vCount + 2, vCount, vCount + 2, vCount + 3 });

            // Use atlas-based UVs for this block type
            AddFaceUVs(type, dir);
        }

        void AddFaceUVs(BlockType type, int dir)
        {
            Vector2Int atlasCoords = BlockUVData.GetAtlasCoords(type, dir);

            float xMin = atlasCoords.x * tileSize + uvPadding;
            float yMin = atlasCoords.y * tileSize + uvPadding;
            float xMax = (atlasCoords.x + 1) * tileSize - uvPadding;
            float yMax = (atlasCoords.y + 1) * tileSize - uvPadding;

            uvs.Add(new Vector2(xMin, yMin));
            uvs.Add(new Vector2(xMax, yMin));
            uvs.Add(new Vector2(xMax, yMax));
            uvs.Add(new Vector2(xMin, yMax));
        }
    }
}