using UnityEngine;
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

        public void SetBGMVolume(float value) => AudioListener.volume = value;
        public void SetSkybox(int index) => EventService.Instance.OnSkyboxChanged.InvokeEvent(index);

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}