using UnityEngine;
using VoxelWorld.Core;
using VoxelWorld.Core.Events;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.OptionsUI
{
    public class OptionsUIController : IUIController
    {
        private OptionsUIView view;

        public OptionsUIController(OptionsUIView view)
        {
            this.view = view;
            this.view.SetController(this);
            Hide();
        }

        public void SetBGMVolume(float value) => GlobalSoundService.MusicVolume = value;
        public void SetSkybox(int index) => EventService.Instance.OnSkyboxChanged.InvokeEvent(index);
        public void OnClose()
        {
            GlobalSoundService.Instance.SoundService.PlaySFX(Sound.SoundType.UI_BUTTON_CLICK);
            GlobalSoundService.SaveVolumes();
            Hide();
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}