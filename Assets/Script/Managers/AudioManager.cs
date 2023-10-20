using System;
using System.Linq;
using UnityEngine;
using static AudioSettings;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource sfx_AudioSource, bg_AudioSource;
    [SerializeField] AudioSettings audioSettings;


    private void OnEnable()
    {
        BaseEvents.OnSoundPlay += OnPlaySound;
    }
    private void OnDisable()
    {
        BaseEvents.OnSoundPlay -= OnPlaySound;
    }



    private void StopSound(AudioData bG_Audio)
    {
        AudioSource source = bG_Audio is SFX_AudioData ? sfx_AudioSource : bg_AudioSource;
        source.Stop();
        source.clip = null;

    }





    private void OnPlaySound(SoundEffectsEnum sfx)
    {
        if (audioSettings == null)
        {
            Debug.LogError("No audio _settings set");
            return;
        }
        var foundSoundData = audioSettings.SFX_AudioDataArray.Where(x => x.Effect == sfx).ToArray();
        if (foundSoundData.Count() <= 0 || foundSoundData.Select(x => x.ClipsArray).Any(y => y.Length <= 0 || y.Any(z => z == null)))
        {
            Debug.LogError($"sfx {sfx} is not set in AudioManager script");
            return;
        }

        var audioData = foundSoundData[UnityEngine.Random.Range(0, foundSoundData.Length)];
        OnPlaySound(audioData);
    }

    private void OnPlaySound(AudioData audioData)
    {
        if (audioData.ClipsArray.Length <= 0 || audioData.ClipsArray.Any(x => x == null))
        {
            Debug.LogError($"audioData clips is not set");
            return;
        }
        int clipIndex = audioData.IsOrderd ? audioData.CurrentOrder++ % audioData.ClipsArray.Length : UnityEngine.Random.Range(0, audioData.ClipsArray.Length);
        AudioClip clip = audioData.ClipsArray[clipIndex];
        //Debug.Log("Playing clip " + clip.name);
        switch (audioData.AudioType)
        {
            case AudioTypeEnum.SFX:
                sfx_AudioSource.clip = clip;
                sfx_AudioSource.Play();
                break;
            case AudioTypeEnum.Music:
                bg_AudioSource.clip = clip;
                bg_AudioSource.Play();
                break;
            case AudioTypeEnum.OneShoot:
                sfx_AudioSource.PlayOneShot(clip);
                break;
            default:
                sfx_AudioSource.PlayOneShot(clip);
                break;
        }
    }


    [ContextMenu("Test sound")]
    void TestSound()
    {
        sfx_AudioSource.Play();
    }
}


public enum SoundEffectsEnum
{
    None,
    Ding,
    Click,
    Win3,
    Lose,
    Win2
}