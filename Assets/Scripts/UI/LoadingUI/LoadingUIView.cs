using TMPro;
using UnityEngine;
using VoxelWorld.UI.Interface;
using System.Collections;

namespace VoxelWorld.UI.LoadingUI
{
    public class LoadingUIView : MonoBehaviour, IUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private LoadingUIController controller;

        public void SetController(IUIController controllerToSet) =>
            controller = controllerToSet as LoadingUIController;

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