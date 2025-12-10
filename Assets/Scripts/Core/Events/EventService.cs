using UnityEngine;
using VoxelWorld.Core.Utilities;

namespace VoxelWorld.Core.Events
{
    public class EventService : GenericMonoSingleton<EventService>
    {
        public EventController<bool> OnStartLoading { get; private set; }
        public EventController<bool> OnGameInitialized { get; private set; }
        public EventController<bool> OnGamePause { get; private set; }
        public EventController<Vector2Int> OnChunkMeshReady { get; private set; }

        public EventService()
        {
            OnStartLoading = new EventController<bool>();
            OnGameInitialized = new EventController<bool>();
            OnGamePause = new EventController<bool>();
            OnChunkMeshReady = new EventController<Vector2Int>();
        }
    }
}