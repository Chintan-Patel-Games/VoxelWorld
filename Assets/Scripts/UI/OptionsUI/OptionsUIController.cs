using UnityEngine;
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

        // --- AUDIO ---
        public void SetBGMVolume(float value) => AudioListener.volume = value;

        // --- SKYBOX ---
        public void SetSkybox(int index)
        {
            if (index < 0 || index >= view.SkyboxMaterials.Length) return;

            RenderSettings.skybox = view.SkyboxMaterials[index];
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}