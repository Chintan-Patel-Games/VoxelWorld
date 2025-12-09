using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Core.Events;
using VoxelWorld.Core.Utilities;
using VoxelWorld.WorldGeneration.Meshes;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkRunner : GenericMonoSingleton<ChunkRunner>
    {
        // Mesh application queue (main thread): (controller, meshData)
        private static readonly Queue<(ChunkController, MeshModel)> pendingMeshApplies = new();
        private static readonly object meshQueueLock = new();

        // Collider assignment queue (main thread): (controller, mesh)
        private static readonly Queue<(ChunkController, Mesh)> pendingColliderApplies = new();
        private static readonly object colliderQueueLock = new();

        private const int meshAppliesPerFrame = 2;
        private const int colliderAppliesPerFrame = 1;

        // StartCoroutine helper
        public static Coroutine Run(IEnumerator routine) => Instance.StartCoroutine(routine);

        // Worker threads call this to enqueue MeshData for application
        public static void EnqueueMeshApply(ChunkController controller, MeshModel meshData)
        {
            if (controller == null || meshData == null) return;

            lock (meshQueueLock)
            {
                pendingMeshApplies.Enqueue((controller, meshData));
            }
        }

        // enqueue collider assignment (main thread)
        public static void EnqueueColliderApply(ChunkController controller, Mesh mesh)
        {
            if (controller == null || mesh == null) return;
            lock (colliderQueueLock)
            {
                pendingColliderApplies.Enqueue((controller, mesh));
            }
        }

        private void Update()
        {
            // Apply up to meshAppliesPerFrame meshes per frame
            for (int i = 0; i < meshAppliesPerFrame; i++)
            {
                (ChunkController controller, MeshModel data) item;
                lock (meshQueueLock)
                {
                    if (pendingMeshApplies.Count == 0) break;
                    item = pendingMeshApplies.Dequeue();
                }
                
                ApplyMesh(item.controller, item.data);
            }

            // Apply up to colliderAppliesPerFrame colliders per frame
            for (int i = 0; i < colliderAppliesPerFrame; i++)
            {
                (ChunkController controller, Mesh mesh) item;
                lock (colliderQueueLock)
                {
                    if (pendingColliderApplies.Count == 0) break;
                    item = pendingColliderApplies.Dequeue();
                }

                ApplyCollider(item.controller, item.mesh);
            }
        }

        // Runs on main thread: create Unity Mesh and assign to MeshFilter (but NOT collider here)
        private void ApplyMesh(ChunkController controller, MeshModel data)
        {
            // If chunk was destroyed, ignore this mesh
            if (!GameService.ChunkService.ActiveChunks.ContainsKey(controller.Coord)) return;
            if (controller.View == null) return;

            // Create Mesh on main thread from MeshData arrays
            Mesh mesh = new Mesh();
            mesh.indexFormat = (data.vertices.Length > 65535) ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.vertices = data.vertices;
            mesh.triangles = data.triangles;
            mesh.uv = data.uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Assign to MeshFilter
            if (controller.View.meshFilter == null) controller.View.meshFilter = controller.View.GetComponent<MeshFilter>();
            if (controller.View.meshFilter == null) return;

            controller.View.meshFilter.sharedMesh = mesh;

            EventService.Instance.OnChunkMeshReady.InvokeEvent(controller.Coord);

            // If chunk requires collider, enqueue collider assignment to be done later
            if (controller.RequiresCollider)
            {
                // Keep mesh reference alive by enqueuing it
                EnqueueColliderApply(controller, mesh);
            }
        }

        // Runs on main thread: assign Mesh to MeshCollider
        private void ApplyCollider(ChunkController controller, Mesh mesh)
        {
            if (controller == null || controller.View == null || mesh == null) return;

            if (controller.View.meshCollider == null) controller.View.meshCollider = controller.View.GetComponent<MeshCollider>();
            if (controller.View.meshCollider != null) controller.View.meshCollider.sharedMesh = mesh;
        }
    }
}