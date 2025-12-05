using UnityEngine.SceneManagement;
using VoxelWorld.Core;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.MainMenuUI
{
    public class MainMenuUIController : IUIController
    {
        private MainMenuUIView view;

        public MainMenuUIController(MainMenuUIView view)
        {
            this.view = view;
            this.view.SetController(this);
            Show();
        }

        public void StartGame() => SceneManager.LoadScene("Voxel Craft");

        public void QuitGame() => GameService.Instance.OnExitGame();

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}