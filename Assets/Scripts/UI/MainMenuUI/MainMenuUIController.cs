using UnityEditor.MPE;
using UnityEngine.SceneManagement;
using VoxelWorld.Core;
using VoxelWorld.UI.Interface;
using VoxelWorld.Core.Events;

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

        public void StartGame()
        {
            Hide();
            UIService.Instance.ShowLoadingUI();
            Core.Events.EventService.Instance.OnStartLoading.InvokeEvent(true);
            SceneManager.LoadScene("VoxelCraft");
        }

        public void ShowOptionsUI() => UIService.Instance.ShowOptionsUI();

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBGL
                Debug.Log("WebGL cannot quit the application.");
            #else
                Application.Quit();
            #endif
        }


        public void Show() => view.EnableView();
        public void Hide() => view.DisableView();
    }
}