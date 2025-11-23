using UnityEngine;

namespace VoxelWorld.WorldGeneration.Chunks
{
    // Lightweight container of arrays (safe to create on worker threads)
    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
    }
}