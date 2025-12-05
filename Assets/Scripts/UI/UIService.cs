using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.UI.MainMenuUI;

namespace VoxelWorld.UI
{
    public class UIService : GenericMonoSingleton<UIService>
    {
        [Tooltip("MainMenuUI")]
        [SerializeField] private MainMenuUIView mainMenuView;
        private MainMenuUIController mainMenuController;

        private void Start()
        {
            mainMenuController = new MainMenuUIController(mainMenuView);
        }
    }
}