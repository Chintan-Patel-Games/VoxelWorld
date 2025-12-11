using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Sound
{
    [CreateAssetMenu(fileName = "SoundScriptableObject", menuName = "ScriptableObjects/SoundSO")]
    public class SoundSO : ScriptableObject
    {
        public Sounds[] audioList;

        public AudioClip[] GetClipsByType(SoundType type)
        {
            List<AudioClip> result = new List<AudioClip>();

            foreach (var s in audioList)
                if (s.soundType == type && s.audio != null)
                    result.Add(s.audio);

            return result.ToArray();
        }
    }

    [System.Serializable]
    public struct Sounds
    {
        public SoundType soundType;
        public AudioClip audio;
    }
}