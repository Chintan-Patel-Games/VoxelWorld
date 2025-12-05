using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.UI.MainMenuUI;
using VoxelWorld.UI.PauseUI;

namespace VoxelWorld.UI
{
    public class UIService : GenericMonoSingleton<UIService>
    {
        [Tooltip("MainMenuUI")]
        [SerializeField] private MainMenuUIView mainMenuView;
        private MainMenuUIController mainMenuController;

        [Tooltip("PauseUI")]
        [SerializeField] private PauseUIView pauseView;
        private PauseUIController pauseController;

        private void Start()
        {
            mainMenuController = new MainMenuUIController(mainMenuView);
            pauseController = new PauseUIController(pauseView);
        }

        public void ShowMainMenuUI() => mainMenuController.Show();
        public void HideMainMenuUI() => mainMenuController.Hide();

        public void ShowPauseUI() => pauseController.Show();
        public void HidePauseUI() => pauseController.Hide();

        public void ShowOptionsUI() { }
        public void HideOptionsUI() { }
    }
}