using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelWorld.Core.Events;
using VoxelWorld.Core.Utilities;
using VoxelWorld.UI;
using VoxelWorld.WorldGeneration.Chunks;
using VoxelWorld.WorldGeneration.World;

namespace VoxelWorld.Core
{
    public class GameService : GenericMonoSingleton<GameService>
    {
        [Header("References")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private CinemachineVirtualCamera cinemachineCam;
        [SerializeField] private WorldController worldController;

        [Header("World Settings")]
        [SerializeField] private float loadDelay = 0.02f;
        [SerializeField] private int worldSeed = 12345;

        // Services
        private Transform player;
        private Vector3 spawnPos;

        public WorldService worldService { get; private set; }
        public TreeService TreeService { get; private set; }

        private bool isGameScene;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;

            // Determine scene type
            isGameScene = SceneManager.GetActiveScene().name == "VoxelCraft";

            if (isGameScene)
            {
                InitializeServices();
                worldController.SetupFog();
            }
        }

        private void InitializeServices()
        {
            worldService = new WorldService(chunkPrefab, worldSeed, loadDelay);
            TreeService = new TreeService(worldSeed);
        }

        private void Start()
        {
            if (!isGameScene) return; // MainMenu scene -> skip world generation

            // Force ChunkRunner to initialize so its Update() will run
            _ = ChunkRunner.Instance;

            if (playerPrefab == null || chunkPrefab == null || cinemachineCam == null || worldController == null)
            {
                Debug.LogError("GameService: Missing reference!");
                enabled = false;
                return;
            }

            // Pick spawn position
            spawnPos = new Vector3(Random.Range(-200, 200), 0, Random.Range(-200, 200));

            EventService.Instance.OnChunkMeshReady.AddListener(OnSpawnChunkMeshReady);

            // Generate initial chunk where player will spawn
            worldService.GenerateInitialChunk(spawnPos);

            // Force mesh build for initial spawn chunk (before streaming/player exist)
            Vector2Int spawnCoord = worldService.WorldToChunkCoord(spawnPos);
            worldService.GetChunkService().BuildChunkMesh(spawnCoord);
        }

        private void OnSpawnChunkMeshReady(Vector2Int coord)
        {
            if (!isGameScene) return;

            Vector2Int spawnCoord = worldService.WorldToChunkCoord(spawnPos);
            
            if (coord != spawnCoord) return;  // Only spawn when THIS EXACT chunk mesh is ready

            // Stop listening (so no duplicate spawns)
            EventService.Instance.OnChunkMeshReady.RemoveListener(OnSpawnChunkMeshReady);

            SpawnPlayerAtSurface();
        }

        private void SpawnPlayerAtSurface()
        {
            int surfaceY = worldService.GetSurfaceHeight(spawnPos);

            Vector3 finalSpawnPos = new Vector3(spawnPos.x, surfaceY + 2f, spawnPos.z);

            // Spawn Player
            GameObject playerObj = Instantiate(playerPrefab, finalSpawnPos, Quaternion.identity);
            player = playerObj.transform;

            // Attach Cinemachine
            Transform followCam = playerObj.transform.Find("PlayerCameraRoot");

            if (followCam != null)
            {
                cinemachineCam.Follow = followCam;
                cinemachineCam.LookAt = followCam;
            }
            else
            {
                Debug.LogWarning("PlayerCameraRoot not found inside player prefab.");
            }

            EventService.Instance.OnGameInitialized.InvokeEvent(true);
            UIService.Instance.HideLoadingUI();

            // Begin streaming chunks around player
            worldService.StartStreamingFromPlayer(player, worldController);
        }

        public static ChunkService ChunkService => Instance.worldService.GetChunkService();

        // ---------------- GAME FLOW CONTROLS -----------------
        public void OnResumeGame() => Time.timeScale = 1f;
        public void OnPauseGame() => Time.timeScale = 0f;
    }
}