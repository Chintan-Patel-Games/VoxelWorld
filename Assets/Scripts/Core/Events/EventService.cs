using UnityEngine;

namespace VoxelWorld.Core.Events
{
    public class EventService
    {
        public EventController<Vector2Int> OnChunkMeshReady { get; private set; }

        public EventService()
        {
            OnChunkMeshReady = new EventController<Vector2Int>();
        }
    }
}