using UnityEngine.SceneManagement;
using VoxelWorld.Core;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.PauseUI
{
    public class PauseUIController : IUIController
    {
        private PauseUIView view;

        public PauseUIController(PauseUIView view)
        {
            this.view = view;
            this.view.SetController(this);
            Show();
        }

        public void ResumeGame() => GameService.Instance.OnResumeGame();
        public void ShowOptionsUI() => GameService.Instance.UIService.ShowOptionsUI();
        public void ShowMainMenuUI() => GameService.Instance.UIService.ShowMainMenuUI();

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}