using UnityEngine;

namespace VoxelWorld.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Camera Target")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        private CharacterController characterController;
        private PlayerController controller;

        private void Awake() => characterController = GetComponent<CharacterController>();

        private void Update() => controller?.TickUpdate(transform);

        private void LateUpdate() => controller?.TickLateUpdate(transform);

        public void SetController(PlayerController controller) => this.controller = controller;

        public Transform CameraTarget => CinemachineCameraTarget.transform;

        public CharacterController CharacterController => characterController;
    }
}