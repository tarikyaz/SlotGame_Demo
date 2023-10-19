using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Settings", menuName = "ScriptableObjects/Audio/Settings", order = 1)]

public class AudioSettings : ScriptableObject
{
    public enum AudioTypeEnum
    {
        None, SFX, Music, OneShoot
    }
    [Serializable]
    public class AudioData
    {
        public AudioTypeEnum AudioType;
        public AudioClip[] ClipsArray = new AudioClip[0];
        public bool IsOrderd;
        [Range(0.0f, 1.0f)] public float Volume = 1;
        internal int CurrentOrder = -1;

    }
    [Serializable]
    public class SFX_AudioData : AudioData
    {
        public SoundEffectsEnum Effect = SoundEffectsEnum.None;
    }

    public SFX_AudioData[] SFX_AudioDataArray = new SFX_AudioData[0];
}
