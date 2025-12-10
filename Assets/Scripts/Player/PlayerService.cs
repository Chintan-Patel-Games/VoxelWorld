using Cinemachine;
using UnityEngine;
using VoxelWorld.Core.Events;
using VoxelWorld.Core.Utilities;
using VoxelWorld.Player;
using VoxelWorld.WorldGeneration.World;

namespace VoxelWorld.Core.PlayerSystem
{
    public class PlayerService : GenericMonoSingleton<PlayerService>
    {
        [Header("Player References")]
        [Tooltip("Reference to the Player Prefab")]
        [SerializeField] private GameObject playerPrefab;

        [Tooltip("Reference to the Player Model")]
        [SerializeField] private PlayerModel model;

        [Tooltip("Reference to the Virtual Camera")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private Transform playerPos;

        public Transform SpawnPlayerAtSurface(Vector3 spawnPosition)
        {
            int surfaceY = GameService.Instance.worldService.GetSurfaceHeight(spawnPosition);
            Vector3 finalSpawnPos = new Vector3(spawnPosition.x, surfaceY + 2f, spawnPosition.z);

            playerPos = SpawnPlayer(finalSpawnPos);
            EventService.Instance.OnGameInitialized.InvokeEvent(true);  // Notify loading system

            return playerPos;
        }

        public Transform SpawnPlayer(Vector3 spawnPos)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            var view = playerObj.GetComponent<PlayerView>();

            PlayerController controller = new PlayerController(
                model,
                view.CharacterController,
                view.CameraTarget
            );

            // Inject controller into view
            view.SetController(controller);

            AttachCamera(view);

            return view.transform;
        }

        private void AttachCamera(PlayerView view)
        {
            Transform followTarget = view.CameraTarget;

            if (followTarget != null)
            {
                virtualCamera.Follow = followTarget;
                virtualCamera.LookAt = followTarget;
            }
            else
            {
                Debug.LogWarning("PlayerCameraRoot not found in player prefab.");
            }
        }

        public Transform GetPlayerPos() => playerPos;
    }
}