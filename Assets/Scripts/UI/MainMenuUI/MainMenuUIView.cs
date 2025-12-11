using UnityEngine;
using UnityEngine.UI;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.MainMenuUI
{
    public class MainMenuUIView : MonoBehaviour, IUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button quitButton;

        private MainMenuUIController controller;

        public void SetController(IUIController controllerToSet)
        {
            controller = controllerToSet as MainMenuUIController;
            SubscribeToButtonClicks();
        }

        private void SubscribeToButtonClicks()
        {
            startButton.onClick.AddListener(controller.StartGame);
            optionButton.onClick.AddListener(controller.ShowOptionsUI);
            quitButton.onClick.AddListener(controller.QuitGame);
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.enabled = true;
            canvasGroup.interactable = true;
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
            canvasGroup.enabled = false;
            canvasGroup.interactable = false;
        }
    }
}