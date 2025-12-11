using UnityEngine;
using UnityEngine.UI;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.PauseUI
{
    public class PauseUIView : MonoBehaviour, IUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButton;

        private PauseUIController controller;

        public void SetController(IUIController controllerToSet)
        {
            controller = controllerToSet as PauseUIController;
            SubscribeToButtonClicks();
        }

        private void SubscribeToButtonClicks()
        {
            resumeButton.onClick.AddListener(controller.ResumeGame);
            optionsButton.onClick.AddListener(controller.ShowOptionsUI);
            mainMenuButton.onClick.AddListener(controller.ShowMainMenuUI);
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