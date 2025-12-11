using UnityEngine;

namespace VoxelWorld.Sound
{
    public class SceneAudioInstaller : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        private void Start()
        {
            GlobalSoundService.Instance.RegisterAudioSources(sfxSource, bgmSource);
        }
    }
}