using System;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkController
    {
        // Neighbor references (4-way adjacency)
        public ChunkController North;
        public ChunkController South;
        public ChunkController East;
        public ChunkController West;

        public Vector2Int Coord { get; private set; }
        public ChunkModel Model { get; private set; }
        public ChunkView View { get; private set; }

        private readonly int chunkSize;
        private readonly int chunkHeight;

        public bool RequiresCollider = false; // Applied by WorldController based on player proximity

        public ChunkController(Vector2Int coord, ChunkView view, int chunkSize = 16, int chunkHeight = 128)
        {
            Coord = coord;
            View = view;
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;
            Model = new ChunkModel(chunkSize, chunkHeight);
        }

        public bool HasMesh => View?.meshFilter?.sharedMesh != null;

        // Generate blocks using provided function local-to-chunk (x,y,z)
        public void GenerateBlocks(Func<int, int, int, BlockType> generator)
        {
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++)
                    for (int y = 0; y < chunkHeight; y++)
                        Model.blocks[x, y, z] = new Block(generator(x, y, z), new Vector3Int(x, y, z));
        }

        public void BuildMesh() => ChunkMeshService.RequestMeshBuild(this);
        public void RebuildMesh() => ChunkMeshService.RequestMeshBuild(this);

        // Apply neighbor: this sets neighbor in direction 'dir' and also wires up the reciprocal link on the neighbor.
        public void ApplyNeighbor(Direction dir, ChunkController neighbor)
        {
            SetNeighbor(dir, neighbor);

            if (neighbor != null)
            {
                Direction opp = Opposite(dir);
                neighbor.SetNeighbor(opp, this);

                // rebuild both sides to hide interior faces
                this.OnNeighborAdded(neighbor);
                neighbor.OnNeighborAdded(this);
            }
            else
            {
                // neighbor removed
                this.OnNeighborRemoved();
            }
        }

        public void SetNeighbor(Direction dir, ChunkController neighbor)
        {
            switch (dir)
            {
                case Direction.North: North = neighbor; break;
                case Direction.South: South = neighbor; break;
                case Direction.East: East = neighbor; break;
                case Direction.West: West = neighbor; break;
            }
        }

        public ChunkController GetNeighbor(Direction dir)
        {
            return dir switch
            {
                Direction.North => North,
                Direction.South => South,
                Direction.East => East,
                Direction.West => West,
                _ => null
            };
        }

        public void OnNeighborAdded(ChunkController neighbor) => RebuildMesh();

        public void OnNeighborRemoved() => RebuildMesh();

        public void RebuildBorders()
        {
            // rebuild this and immediate neighbors (so borders update correctly)
            RebuildMesh();
            North?.RebuildMesh();
            South?.RebuildMesh();
            East?.RebuildMesh();
            West?.RebuildMesh();
        }

        private Direction Opposite(Direction d)
        {
            return d switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => d,
            };
        }

        // Get block within this chunk or across neighbor boundary (local coords may be outside [0..size-1])
        public Block GetNeighborBlock(int localX, int localY, int localZ)
        {
            int size = chunkSize;
            int height = chunkHeight;

            // internal
            if (localX >= 0 && localX < size &&
                localY >= 0 && localY < height &&
                localZ >= 0 && localZ < size)
            {
                return Model.blocks[localX, localY, localZ];
            }

            // west (-x)
            if (localX < 0)
            {
                if (West == null) return null;
                return West.Model.blocks[size - 1, localY, localZ];
            }

            // east (+x)
            if (localX >= size)
            {
                if (East == null) return null;
                return East.Model.blocks[0, localY, localZ];
            }

            // south (-z)
            if (localZ < 0)
            {
                if (South == null) return null;
                return South.Model.blocks[localX, localY, size - 1];
            }

            // north (+z)
            if (localZ >= size)
            {
                if (North == null) return null;
                return North.Model.blocks[localX, localY, 0];
            }

            return null;
        }
    }
}