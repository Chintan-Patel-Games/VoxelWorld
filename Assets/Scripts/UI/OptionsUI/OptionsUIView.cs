using UnityEngine;
using UnityEngine.UI;
using VoxelWorld.UI.Interface;

namespace VoxelWorld.UI.OptionsUI
{
    public class OptionsUIView : MonoBehaviour, IUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Skyboxes")]
        [SerializeField] private Material[] skyboxMaterials;

        [Header("Audio")]
        [SerializeField] private Slider bgmSlider;

        [Header("Button")]
        [SerializeField] private Button backBtn;

        private OptionsUIController controller;

        public Material[] SkyboxMaterials { get; }

        public void SetController(IUIController controllerToSet)
        {
            controller = controllerToSet as OptionsUIController;
            SubscribeToSliders();
            SubscribeToButtons();
        }

        private void SubscribeToSliders() => bgmSlider.onValueChanged.AddListener(controller.SetBGMVolume);

        private void SubscribeToButtons() => backBtn.onClick.AddListener(controller.Hide);

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