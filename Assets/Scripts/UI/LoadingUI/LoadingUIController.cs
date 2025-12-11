using UnityEngine;
using UnityEngine.UI;
using VoxelWorld.Core;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.LoadingUI
{
    public class LoadingUIController : IUIController
    {
        private LoadingUIView view;

        public LoadingUIController(LoadingUIView view)
        {
            this.view = view;
            this.view.SetController(this);
            Hide();
        }

        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}