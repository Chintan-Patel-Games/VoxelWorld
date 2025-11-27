using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Chunks;

namespace VoxelWorld.WorldGeneration.Meshes
{
    public static class MeshController
    {
        // Use Vector3 for normals (Mesh.normals expects Vector3)
        private static readonly Vector3Int[] faceNormal =
        {
            new Vector3Int(0, 0, 1),   // north  (z+)
            new Vector3Int(0, 0, -1),  // south  (z-)
            new Vector3Int(-1, 0, 0),  // west   (x-)
            new Vector3Int(1, 0, 0),   // east   (x+)
            new Vector3Int(0, 1, 0),   // up     (y+)
            new Vector3Int(0, -1, 0),  // down   (y-)
        };

        // Each face orientation has its own quad vertices
        private static readonly Vector3[,] faceVerts = new Vector3[6, 4]
        {
            // north (+z)
            { new(0,0,1), new(1,0,1), new(1,1,1), new(0,1,1) },

            // south (-z)
            { new(1,0,0), new(0,0,0), new(0,1,0), new(1,1,0) },

            // west (-x)
            { new(0,0,0), new(0,0,1), new(0,1,1), new(0,1,0) },

            // east (+x)
            { new(1,0,1), new(1,0,0), new(1,1,0), new(1,1,1) },

            // up (+y)
            { new(0,1,1), new(1,1,1), new(1,1,0), new(0,1,0) },

            // down (-y)
            { new(0,0,0), new(1,0,0), new(1,0,1), new(0,0,1) }
        };

        private const int ATLAS_GRID = 5;      // 5x5 atlas (adjust if yours is different)
        private const float uvPadding = 0.001f;

        public static MeshModel GenerateMeshData(ChunkController chunk)
        {
            var model = chunk.Model;
            int sizeX = model.blocks.GetLength(0);
            int sizeY = model.blocks.GetLength(1);
            int sizeZ = model.blocks.GetLength(2);

            List<Vector3> verts = new(4096);
            List<int> tris = new(8192);
            List<Vector2> uvs = new(4096);
            List<Vector3> normals = new(4096);

            // Loop through all blocks
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    for (int z = 0; z < sizeZ; z++)
                    {
                        Block block = model.blocks[x, y, z];
                        if (block == null || block.blockType == BlockType.Air)
                            continue;

                        Vector3Int pos = new Vector3Int(x, y, z);

                        // Check each of the 6 faces
                        for (int face = 0; face < 6; face++)
                        {
                            Vector3Int nPos = pos + (Vector3Int)faceNormal[face];
                            Block neighbor = chunk.GetNeighborBlock(nPos.x, nPos.y, nPos.z);

                            bool solidNeighbor = neighbor != null && neighbor.blockType != BlockType.Air;

                            if (!solidNeighbor)
                            {
                                AddQuad(face, pos, block.blockType, verts, tris, uvs, normals);
                            }
                        }
                    }

            return new MeshModel
            {
                vertices = verts.ToArray(),
                triangles = tris.ToArray(),
                uvs = uvs.ToArray(),
            };
        }

        private static void AddQuad(
            int face,
            Vector3Int blockPos,
            BlockType type,
            List<Vector3> verts,
            List<int> tris,
            List<Vector2> uvs,
            List<Vector3> normals)
        {
            int start = verts.Count;

            // Add 4 vertices
            for (int i = 0; i < 4; i++)
                verts.Add(blockPos + faceVerts[face, i]);

            // Add 4 normals for this face
            Vector3 n = faceNormal[face];
            normals.Add(n);
            normals.Add(n);
            normals.Add(n);
            normals.Add(n);

            // Triangles
            tris.Add(start + 0);
            tris.Add(start + 1);
            tris.Add(start + 2);
            tris.Add(start + 0);
            tris.Add(start + 2);
            tris.Add(start + 3);

            // Atlas UVs
            Vector2Int atlas = BlockUVData.GetAtlasCoords(type, face);
            float tileSize = 1f / ATLAS_GRID;

            float u0 = atlas.x * tileSize + uvPadding;
            float v0 = atlas.y * tileSize + uvPadding;
            float u1 = (atlas.x + 1) * tileSize - uvPadding;
            float v1 = (atlas.y + 1) * tileSize - uvPadding;

            uvs.Add(new Vector2(u0, v0));
            uvs.Add(new Vector2(u1, v0));
            uvs.Add(new Vector2(u1, v1));
            uvs.Add(new Vector2(u0, v1));
        }
    }
}