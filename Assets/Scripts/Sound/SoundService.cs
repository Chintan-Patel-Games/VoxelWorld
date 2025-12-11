using System;
using UnityEngine;

namespace VoxelWorld.Sound
{
    public class SoundService
    {
        private SoundSO soundSO;

        private AudioSource sfxSource;
        private AudioSource bgmSource;

        private AudioClip[] footstepClips;
        private int footstepIndex = 0;
        private bool footstepsPlaying = false;

        private float walkInterval = 0.5f; // walking interval
        private float sprintInterval = 0.3f; // sprint = 30% faster
        private float footstepInterval;
        private float footstepTimer = 0f;

        public SoundService(SoundSO soundSO) => this.soundSO = soundSO;

        public void AssignAudioSources(AudioSource sfx, AudioSource bgm)
        {
            sfxSource = sfx;
            bgmSource = bgm;

            footstepClips = soundSO.GetClipsByType(SoundType.WALKING_SOUND);

            PlayBGM(SoundType.BACKGROUND_MUSIC, true);
        }

        public void PlaySFX(SoundType soundType, bool loopSound = false)
        {
            AudioClip clip = GetSoundClip(soundType);
            if (clip != null)
            {
                sfxSource.loop = loopSound;
                sfxSource.clip = clip;
                sfxSource.PlayOneShot(clip);
            }
            else
                Debug.LogError("No Audio Clip selected.");
        }

        private void PlayBGM(SoundType soundType, bool loopSound = false)
        {
            AudioClip clip = GetSoundClip(soundType);
            if (clip != null)
            {
                bgmSource.loop = loopSound;
                bgmSource.clip = clip;
                bgmSource.Play();
            }
            else
                Debug.LogError("No Audio Clip selected.");
        }

        public void StartFootsteps(bool isSprinting)
        {
            if (footstepsPlaying) return; // <-- prevents duplicate loops !!

            footstepsPlaying = true;

            // Update step interval instantly
            footstepInterval = isSprinting ? sprintInterval : walkInterval;

            // Reset sequence so it starts immediately in new mode
            ResetFootsteps();
        }

        public void StopFootsteps()
        {
            if (!footstepsPlaying) return;

            footstepsPlaying = false;
            footstepTimer = 0f;
        }

        public void UpdateFootstepsMode(bool isSprinting)
        {
            // Update interval without restarting loop
            footstepInterval = isSprinting ? sprintInterval : walkInterval;
        }

        public void UpdateFootsteps(float deltaTime)
        {
            if (!footstepsPlaying || footstepClips == null || footstepClips.Length == 0)
                return;

            footstepTimer += deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayNextFootstep();
                footstepTimer = 0f;
            }
        }

        private void PlayNextFootstep()
        {
            if (footstepClips.Length == 0) return;

            AudioClip clip = footstepClips[footstepIndex];
            sfxSource.PlayOneShot(clip);

            footstepIndex++;
            if (footstepIndex >= footstepClips.Length)
                footstepIndex = 0;
        }

        public void ResetFootsteps()
        {
            footstepIndex = 0;
            footstepTimer = 0f;
        }

        public float MusicVolume
        {
            get => bgmSource != null ? bgmSource.volume : 0f;
            set
            {
                if (bgmSource != null)
                    bgmSource.volume = value;
            }
        }

        public float SFXVolume
        {
            get => sfxSource != null ? sfxSource.volume : 0f;
            set
            {
                if (sfxSource != null)
                    sfxSource.volume = value;
            }
        }

        private AudioClip GetSoundClip(SoundType soundType)
        {
            Sounds sound = Array.Find(soundSO.audioList, item => item.soundType == soundType);
            if (sound.audio != null)
                return sound.audio;
            return null;
        }
    }
}