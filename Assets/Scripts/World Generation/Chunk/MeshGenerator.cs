using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    // Pure CPU generator. MUST NOT call any Unity object methods that touch the engine.
    public static class MeshGenerator
    {
        private static readonly Vector3Int[] faceDirections = ChunkMeshService.faceDirections;
        private const float tileSize = 1f / 5f;
        private const float uvPadding = 0.001f;

        public static MeshData GenerateMeshData(ChunkController controller)
        {
            var model = controller.Model;
            int sizeX = model.blocks.GetLength(0);
            int sizeY = model.blocks.GetLength(1);
            int sizeZ = model.blocks.GetLength(2);

            var verts = new List<Vector3>(4096);
            var tris = new List<int>(8192);
            var uvs = new List<Vector2>(4096);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        Block b = model.blocks[x, y, z];
                        if (b == null || b.blockType == BlockType.Air) continue;

                        Vector3 basePos = new Vector3(x, y, z);

                        for (int f = 0; f < faceDirections.Length; f++)
                        {
                            Vector3Int dir = faceDirections[f];
                            int nx = x + dir.x;
                            int ny = y + dir.y;
                            int nz = z + dir.z;

                            bool neighborSolid = false;

                            // neighbor inside same chunk
                            if (nx >= 0 && nx < sizeX &&
                                ny >= 0 && ny < sizeY &&
                                nz >= 0 && nz < sizeZ)
                            {
                                Block neighbourBlock = model.blocks[nx, ny, nz];
                                neighborSolid = (neighbourBlock != null && neighbourBlock.blockType != BlockType.Air);
                            }
                            else
                            {
                                // cross-chunk lookup via controller (safe: only reads arrays)
                                Block neighbourBlock = controller.GetNeighborBlock(nx, ny, nz);
                                neighborSolid = (neighbourBlock != null && neighbourBlock.blockType != BlockType.Air);
                            }

                            if (!neighborSolid)
                                AddFace(f, basePos, b.blockType, verts, tris, uvs);
                        }
                    }
                }
            }

            // convert to arrays
            var md =  new MeshData
            {
                vertices = verts.ToArray(),
                triangles = tris.ToArray(),
                uvs = uvs.ToArray()
            };
            return md;
        }

        private static void AddFace(int dir, Vector3 pos, BlockType type, List<Vector3> verts, List<int> tris, List<Vector2> uvs)
        {
            int vCount = verts.Count;
            Vector3[] faceVerts = new Vector3[4];

            switch (dir)
            {
                case 0: faceVerts = new[] { pos + new Vector3(0, 0, 1), pos + new Vector3(1, 0, 1), pos + new Vector3(1, 1, 1), pos + new Vector3(0, 1, 1) }; break;
                case 1: faceVerts = new[] { pos + new Vector3(1, 0, 0), pos + new Vector3(0, 0, 0), pos + new Vector3(0, 1, 0), pos + new Vector3(1, 1, 0) }; break;
                case 2: faceVerts = new[] { pos + new Vector3(0, 0, 0), pos + new Vector3(0, 0, 1), pos + new Vector3(0, 1, 1), pos + new Vector3(0, 1, 0) }; break;
                case 3: faceVerts = new[] { pos + new Vector3(1, 0, 1), pos + new Vector3(1, 0, 0), pos + new Vector3(1, 1, 0), pos + new Vector3(1, 1, 1) }; break;
                case 4: faceVerts = new[] { pos + new Vector3(0, 1, 1), pos + new Vector3(1, 1, 1), pos + new Vector3(1, 1, 0), pos + new Vector3(0, 1, 0) }; break;
                case 5: faceVerts = new[] { pos + new Vector3(0, 0, 0), pos + new Vector3(1, 0, 0), pos + new Vector3(1, 0, 1), pos + new Vector3(0, 0, 1) }; break;
            }

            verts.AddRange(faceVerts);
            tris.AddRange(new int[] { vCount, vCount + 1, vCount + 2, vCount, vCount + 2, vCount + 3 });

            var atlasCoords = BlockUVData.GetAtlasCoords(type, dir);
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