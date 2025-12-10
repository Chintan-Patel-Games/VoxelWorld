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
        }

        public void ShowMainMenuUI() => mainMenuController.Show();

        public void ShowLoadingUI() => loadingController.Show();
        public void HideLoadingUI() => loadingController.Hide();

        public void ShowPauseUI() => pauseController.Show();
        public void HidePauseUI() => pauseController.Hide();

        public void ShowOptionsUI() => optionsController.Show();

        public void OnGamePause(bool paused)
        {
            Debug.Log("UIService received pause event: " + paused);

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
    }
}