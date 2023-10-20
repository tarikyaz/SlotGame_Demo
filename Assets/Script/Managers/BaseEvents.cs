using System;
using UnityEngine;
using static AudioSettings;

public static class BaseEvents
{
    public static Action<SoundEffectsEnum> OnSoundPlay;
    public static Action<int> OnCoinsAmountChange;
    public static void CallSoundPlay(SoundEffectsEnum audioType)
    {
        OnSoundPlay?.Invoke(audioType);
    }

    internal static void CallCoinsAmountChange(int value)
    {
        OnCoinsAmountChange?.Invoke(value);
    }
}
