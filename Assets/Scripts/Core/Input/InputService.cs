using UnityEngine;
using VoxelWorld.Core.Utilities;

namespace VoxelWorld.Core.InputSystem
{
    public class InputService : GenericMonoSingleton<InputService>
    {
        // Movement
        public Vector2 Move { get; set; }
        public Vector2 Look { get; set; }

        // Actions
        public bool Jump { get; set; }
        public bool Sprint { get; set; }
        public bool PausePressed { get; set; }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}