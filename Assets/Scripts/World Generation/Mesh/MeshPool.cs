using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.WorldGeneration.Meshes
{
    public static class MeshPool
    {
        private static readonly Stack<Mesh> pool = new Stack<Mesh>();

        public static Mesh Get()
        {
            if (pool.Count > 0)
            {
                Mesh m = pool.Pop();
                m.Clear(false);     // keep GPU buffers
                return m;
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            return mesh;
        }

        public static void Release(Mesh mesh)
        {
            if (mesh == null) return;

            mesh.Clear(false);
            pool.Push(mesh);
        }
    }
}