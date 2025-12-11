using UnityEngine;

namespace VoxelWorld.Core.InputSystem
{
    public class InputModel
    {
        // Movement
        public Vector2 Move { get; set; }
        public Vector2 Look { get; set; }

        // Actions
        public bool Jump { get; set; }
        public bool Sprint { get; set; }
        public bool PausePressed { get; set; }

        // Curson Settings
        public bool CursorLocked { get; set; }
        public bool CursorInputForLook { get; set; }
    }
}