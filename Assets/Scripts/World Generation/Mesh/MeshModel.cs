using UnityEngine;

namespace VoxelWorld.WorldGeneration.Meshes
{
    // Lightweight container of arrays (safe to create on worker threads)
    public class MeshModel
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public Vector3[] normals;
    }
}