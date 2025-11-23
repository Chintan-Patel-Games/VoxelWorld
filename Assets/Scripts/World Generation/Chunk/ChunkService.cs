using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkService
    {
        public Dictionary<Vector2Int, ChunkController> ActiveChunks { get; private set; } = new();
        private readonly Dictionary<Vector2Int, Coroutine> generatingChunks = new();
        private GameObject chunkPrefab;

        public static int chunkSize = 16;
        public static int chunkHeight = 128;

        public ChunkService(GameObject prefab) => chunkPrefab = prefab;

        public bool HasChunk(Vector2Int coord) => ActiveChunks.ContainsKey(coord);

        public ChunkController GetChunk(Vector2Int coord)
        {
            ActiveChunks.TryGetValue(coord, out var c);
            return c;
        }

        public void GenerateChunk(
            Vector2Int coord,
            float delay,
            Func<int, int, int, BlockType> terrainGenerator,
            Action<ChunkController> onChunkReady)
        {
            if (HasChunk(coord) || generatingChunks.ContainsKey(coord))
                return;

            Coroutine routine = ChunkRunner.Run(GenerateRoutine(coord, delay, terrainGenerator, onChunkReady));
            generatingChunks[coord] = routine;
        }

        private IEnumerator GenerateRoutine(Vector2Int coord,
                                            float delay,
                                            Func<int, int, int, BlockType> terrainFunc,
                                            Action<ChunkController> onChunkReady)
        {
            // Instantiate prefab in world space (y = 0)
            Vector3 pos = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
            GameObject viewObj = GameObject.Instantiate(chunkPrefab, pos, Quaternion.identity);
            ChunkView view = viewObj.GetComponent<ChunkView>();

            // Create Controller + Model
            ChunkController controller = new ChunkController(coord, view);
            ActiveChunks[coord] = controller;

            // Fill blocks via controller method
            controller.GenerateBlocks(terrainFunc);

            // small yield if chunk-heavy; respect requested delay between rows to avoid stutter
            if (delay > 0f) yield return new WaitForSeconds(delay);

            // Link neighbors (wire up adjacency)
            TryLinkNeighbors(coord, controller);

            // Additional generation step (trees, models)
            onChunkReady?.Invoke(controller);

            // Generation finished
            if (generatingChunks.ContainsKey(coord))
                generatingChunks.Remove(coord);
        }

        // Attempt to link neighbors in ActiveChunks
        private void TryLinkNeighbors(Vector2Int coord, ChunkController current)
        {
            // north = +z
            Vector2Int north = coord + new Vector2Int(0, 1);
            if (ActiveChunks.TryGetValue(north, out var n))
                current.ApplyNeighbor(Direction.North, n);

            // south = -z
            Vector2Int south = coord + new Vector2Int(0, -1);
            if (ActiveChunks.TryGetValue(south, out var s))
                current.ApplyNeighbor(Direction.South, s);

            // east = +x
            Vector2Int east = coord + new Vector2Int(1, 0);
            if (ActiveChunks.TryGetValue(east, out var e))
                current.ApplyNeighbor(Direction.East, e);

            // west = -x
            Vector2Int west = coord + new Vector2Int(-1, 0);
            if (ActiveChunks.TryGetValue(west, out var w))
                current.ApplyNeighbor(Direction.West, w);
        }

        public void BuildChunkMesh(Vector2Int coord)
        {
            if (!ActiveChunks.TryGetValue(coord, out var controller)) return;

            // If mesh already exists, skip
            //if (controller.View != null && controller.View.meshFilter != null && controller.View.meshFilter.sharedMesh != null)
            //    return;

            controller.BuildMesh();
        }

        // Unload mesh for a chunk but keep the chunk data (so it can be re-meshed quickly)
        public void UnloadChunkMesh(Vector2Int coord)
        {
            if (!ActiveChunks.TryGetValue(coord, out var controller)) return;
            if (controller.View == null) return;

            // Remove collider first if any
            if (controller.View.meshCollider != null)
            {
                controller.View.meshCollider.sharedMesh = null;
            }

            // Remove mesh from MeshFilter and destroy Mesh object to free memory
            if (controller.View.meshFilter != null)
            {
                Mesh m = controller.View.meshFilter.sharedMesh;
                controller.View.meshFilter.sharedMesh = null;

                if (m != null)
                {
                    // Destroy the runtime Mesh object
                    UnityEngine.Object.Destroy(m);
                }
            }
        }

        // Completely destroy a chunk (used when outside LOAD_RADIUS)
        public void DestroyChunk(Vector2Int coord)
        {
            if (!ActiveChunks.TryGetValue(coord, out var chunk)) return;

            // Unlink neighbors
            if (chunk.North != null) chunk.North.ApplyNeighbor(Direction.South, null);
            if (chunk.South != null) chunk.South.ApplyNeighbor(Direction.North, null);
            if (chunk.East != null) chunk.East.ApplyNeighbor(Direction.West, null);
            if (chunk.West != null) chunk.West.ApplyNeighbor(Direction.East, null);

            // Unload mesh safely
            if (chunk.View != null)
            {
                // Null collider first, then destroy mesh/filter and GameObject
                if (chunk.View.meshCollider != null)
                    chunk.View.meshCollider.sharedMesh = null;

                if (chunk.View.meshFilter != null)
                {
                    Mesh m = chunk.View.meshFilter.sharedMesh;
                    chunk.View.meshFilter.sharedMesh = null;
                    if (m != null)
                        UnityEngine.Object.Destroy(m);
                }

                UnityEngine.Object.Destroy(chunk.View.gameObject);
            }

            ActiveChunks.Remove(coord);
        }

        public bool IsGenerating(Vector2Int coord) => generatingChunks.ContainsKey(coord);
    }
}