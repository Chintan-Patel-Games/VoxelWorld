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
            Hide();
        }

        public void ResumeGame() => GameService.Instance.OnResumeGame();
        public void ShowOptionsUI() => UIService.Instance.ShowOptionsUI();
        public void ShowMainMenuUI()
        {
            Hide();
            UIService.Instance.ShowMainMenuUI();
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}