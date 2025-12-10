using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Core.Events;
using VoxelWorld.Core.Utilities;
using VoxelWorld.UI.LoadingUI;
using VoxelWorld.UI.MainMenuUI;
using VoxelWorld.UI.OptionsUI;
using VoxelWorld.UI.PauseUI;

namespace VoxelWorld.UI
{
    public class UIService : GenericMonoSingleton<UIService>
    {
        [Tooltip("MainMenuUI")]
        [SerializeField] private MainMenuUIView mainMenuView;
        private MainMenuUIController mainMenuController;

        [Tooltip("LoadingUI")]
        [SerializeField] private LoadingUIView loadingView;
        private LoadingUIController loadingController;

        [Tooltip("PauseUI")]
        [SerializeField] private PauseUIView pauseView;
        private PauseUIController pauseController;

        [Tooltip("OptionsUI")]
        [SerializeField] private OptionsUIView optionsView;
        private OptionsUIController optionsController;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            mainMenuController = new MainMenuUIController(mainMenuView);
            loadingController = new LoadingUIController(loadingView);
            pauseController = new PauseUIController(pauseView);
            optionsController = new OptionsUIController(optionsView);

            EventService.Instance.OnGamePause.AddListener(OnGamePause);
            EventService.Instance.OnSkyboxChanged.AddListener(ApplySkybox);
        }

        public void ShowMainMenuUI() => mainMenuController.Show();

        public void ShowLoadingUI() => loadingController.Show();
        public void HideLoadingUI() => loadingController.Hide();

        public void ShowPauseUI() => pauseController.Show();
        public void HidePauseUI() => pauseController.Hide();

        public void ShowOptionsUI() => optionsController.Show();

        public void OnGamePause(bool paused)
        {
            if (paused)
            {
                ShowPauseUI();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                HidePauseUI();
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void ApplySkybox(int index)
        {
            // Get OptionsUI View to access skybox material array
            if (optionsController == null) return;
            if (optionsView == null)
            {
                Debug.Log("[UIService] : No reference of Options View found");
                return;
            }

            var mats = optionsView.SkyboxMaterials;

            if (index < 0 || index >= mats.Length)
            {
                Debug.LogWarning("[UIService] Invalid skybox index: " + index);
                return;
            }

            RenderSettings.skybox = mats[index];
            DynamicGI.UpdateEnvironment(); // refresh lighting
        }
    }
}