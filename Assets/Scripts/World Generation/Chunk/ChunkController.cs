using System;
using UnityEngine;
using VoxelWorld.WorldGeneration.Blocks;
using VoxelWorld.WorldGeneration.Meshes;

namespace VoxelWorld.WorldGeneration.Chunks
{
    public class ChunkController
    {
        // Neighbor references (4-way adjacency)
        public ChunkController North, South, East, West;

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

        public void BuildMesh() => MeshService.RequestMeshBuild(this);
        public void RebuildMesh() => MeshService.RequestMeshBuild(this);

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
        public Block GetNeighborBlock(int x, int y, int z)
        {
            int size = ChunkService.chunkSize;
            int height = ChunkService.chunkHeight;

            // Out of vertical range = air
            if (y < 0 || y >= height)
                return null;

            // Inside this chunk
            if (x >= 0 && x < size && z >= 0 && z < size)
                return Model.blocks[x, y, z];

            // West
            if (x < 0 && West != null)
                return West.Model.blocks[x + size, y, z];

            // East
            if (x >= size && East != null)
                return East.Model.blocks[x - size, y, z];

            // South
            if (z < 0 && South != null)
                return South.Model.blocks[x, y, z + size];

            // North
            if (z >= size && North != null)
                return North.Model.blocks[x, y, z - size];

            // No neighbor chunk loaded
            return null;
        }

        /// <summary>
        /// Returns the highest non-air block at (x,z) in THIS chunk.
        /// Used by mesher to detect underground.
        /// </summary>
        public int GroundLevelAt(int x, int z)
        {
            int size = ChunkService.chunkSize;
            int height = ChunkService.chunkHeight;

            // Inside bounds
            if (x >= 0 && x < size && z >= 0 && z < size)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    Block b = Model.blocks[x, y, z];
                    if (b != null && b.blockType != BlockType.Air && b.blockType != BlockType.Wood && b.blockType != BlockType.Leaves)
                        return y;

                }
                return 0;
            }

            if (x < 0 && West != null) return West.GroundLevelAt(x + size, z);  // West
            if (x >= size && East != null) return East.GroundLevelAt(x - size, z);  // East
            if (z < 0 && South != null) return South.GroundLevelAt(x, z + size);  // South
            if (z >= size && North != null) return North.GroundLevelAt(x, z - size);  // North
            return 0;
        }

        public Block GetBlockWorld(int x, int y, int z)
        {
            int size = ChunkService.chunkSize;
            int height = ChunkService.chunkHeight;

            // inside this chunk
            if (x >= 0 && x < size && z >= 0 && z < size)
                return Model.blocks[x, y, z];

            // check neighbor chunks
            if (x < 0 && West != null)
                return West.Model.blocks[x + size, y, z];

            if (x >= size && East != null)
                return East.Model.blocks[x - size, y, z];

            if (z < 0 && South != null)
                return South.Model.blocks[x, y, z + size];

            if (z >= size && North != null)
                return North.Model.blocks[x, y, z - size];

            return null; // outside world
        }
    }
}