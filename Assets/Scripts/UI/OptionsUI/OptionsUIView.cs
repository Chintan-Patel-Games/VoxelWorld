using System.Collections.Generic;
using TMPro;
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

        [Header("Dropdown")]
        [SerializeField] private TMP_Dropdown skyboxDropdown;

        [Header("Audio")]
        [SerializeField] private Slider bgmSlider;

        [Header("Button")]
        [SerializeField] private Button backBtn;

        private OptionsUIController controller;

        public Material[] SkyboxMaterials => skyboxMaterials;

        public void SetController(IUIController controllerToSet)
        {
            controller = controllerToSet as OptionsUIController;
            SubscribeToSliders();
            SubscribeToButtons();
            SubscribeToDropdown();
        }

        private void SubscribeToSliders() => bgmSlider.onValueChanged.AddListener(controller.SetBGMVolume);

        private void SubscribeToDropdown()
        {
            // Fill dropdown options with material names
            List<string> skyboxNames = new List<string>();

            foreach (var mat in skyboxMaterials)
            {
                skyboxNames.Add(mat != null ? mat.name : "Unknown");
            }

            skyboxDropdown.ClearOptions();
            skyboxDropdown.AddOptions(skyboxNames);

            // Register callback
            skyboxDropdown.onValueChanged.AddListener(OnSkyboxSelected);
        }

        private void SubscribeToButtons() => backBtn.onClick.AddListener(controller.OnClose);

        public void OnSkyboxSelected(int index) => controller.SetSkybox(index);

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