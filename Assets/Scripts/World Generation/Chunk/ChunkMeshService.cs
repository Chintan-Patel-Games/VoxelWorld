using System.Threading.Tasks;
using UnityEngine;
using VoxelWorld.Core;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public static class ChunkMeshService
    {
        // reuse faceDirections publicly for MeshGenerator
        public static readonly Vector3Int[] faceDirections = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),   // front (z+)
            new Vector3Int(0, 0, -1),  // back  (z-)
            new Vector3Int(-1, 0, 0),  // left  (x-)
            new Vector3Int(1, 0, 0),   // right (x+)
            new Vector3Int(0, 1, 0),   // up    (y+)
            new Vector3Int(0, -1, 0)   // down  (y-)
        };

        // Request async mesh build (runs on worker thread)
        public static void RequestMeshBuild(ChunkController controller)
        {
            if (controller == null) return;
            if (!GameService.ChunkService.HasChunk(controller.Coord)) return; // chunk destroyed while meshing

            // Kick off background task - don't touch UnityEngine objects in the worker
            Task.Run(() =>
            {
                MeshData data = MeshGenerator.GenerateMeshData(controller);

                // Enqueue for main-thread application (ChunkRunner will budget the applies)
                ChunkRunner.EnqueueMeshApply(controller, data);
            });
        }
    }
}