using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Core.Events;
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

        public void ResumeGame()
        {
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            EventService.Instance.OnGamePause.InvokeEvent(false);
        }

        public void ShowOptionsUI()
        {
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            UIService.Instance.ShowOptionsUI();
        }

        public void ShowMainMenuUI()
        {
            Hide();
            Time.timeScale = 1f;
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            UIService.Instance.ShowMainMenuUI();
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}