using System;
using UnityEngine;
using static AudioSettings;

public static class BaseEvents
{
    public static Action<SoundEffectsEnum> OnSoundPlay;
    public static void CallSoundPlay(SoundEffectsEnum audioType)
    {
        OnSoundPlay?.Invoke(audioType);
    }
}
