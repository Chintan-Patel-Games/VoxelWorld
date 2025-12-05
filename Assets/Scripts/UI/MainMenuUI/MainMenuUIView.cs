using UnityEngine;
using UnityEngine.UI;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.MainMenuUI
{
    public class MainMenuUIView : MonoBehaviour, IUIView
    {
        public Button startButton;
        public Button quitButton;

        private MainMenuUIController controller;

        public void SetController(IUIController controllerToSet)
        {
            controller = controllerToSet as MainMenuUIController;
            SubscribeToButtonClicks();
        }

        private void SubscribeToButtonClicks()
        {
            startButton.onClick.AddListener(controller.StartGame);
            quitButton.onClick.AddListener(controller.QuitGame);
        }

        public void EnableView() => gameObject.SetActive(true);
        public void DisableView() => gameObject.SetActive(false);
    }
}