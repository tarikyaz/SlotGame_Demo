using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] GameObject testButtons;
    int timesSpinning
    {
        get => PlayerPrefs.GetInt("TIME_SPININNG_KEY", 0);
        set => PlayerPrefs.SetInt("TIME_SPININNG_KEY", value);
    }
    Reel[] reelsArray;
    bool isRoling => reelsArray.Any(x => x.isRoling);
    Coroutine waitForFinish;
    [Serializable]
    struct ReelData
    {
        public int reelIndex, reelValue;
    }
    ReelData[] reelResultsArray;
    private void Start()
    {
        testButtons.SetActive(false);
        reelsArray = GetComponentsInChildren<Reel>();
    }
    private void OnEnable()
    {
        SlotButton.OnClick += OnSlotButtonClickHandler;
    }
    private void OnDisable()
    {
        SlotButton.OnClick -= OnSlotButtonClickHandler;
    }

    private void OnSlotButtonClickHandler(int index)
    {
        if (!isRoling)
        {
            reelResultsArray = new ReelData[reelsArray.Length];
            if (waitForFinish != null)
            {
                StopCoroutine(waitForFinish);
            }
            BaseEvents.CallSoundPlay(SoundEffectsEnum.Click);
            timesSpinning++;
            GetResult(out var isWin);
            for (int i = 0; i < reelsArray.Length; i++)
            {
                Reel reel = reelsArray[i];
                if (!isWin)
                {
                    index = GetDifferentIndex(index);
                }
                reel.RoleToIndex(index);
                reelResultsArray[i] = new ReelData
                {
                    
                    reelIndex = reel.index,
                    reelValue = index
                };
            }
            bool hasDuplicates = reelResultsArray.GroupBy(x => x.reelValue)
                           .Any(group => group.Count() > 1);
            waitForFinish = StartCoroutine(WaitForAllFinishRoling(() =>
            {
                if (isWin)
                {
                    BaseEvents.CallSoundPlay(SoundEffectsEnum.Win);
                }
                else
                {
                    BaseEvents.CallSoundPlay(SoundEffectsEnum.Lose);
                }
                if (hasDuplicates)
                {
                    foreach (var group in reelResultsArray.GroupBy(x => x.reelValue).Where(x => x.Count() > 1))
                    {
                        foreach (var reelData in group)
                        {
                            reelsArray[reelData.reelIndex].SetColor(Color.red);
                        }
                    }
                }

            }));
        }
    }
    IEnumerator WaitForAllFinishRoling(Action onComplete)
    {
        yield return new WaitUntil(() => isRoling == false);
        onComplete?.Invoke();
    }
    private void GetResult(out bool isWin)
    {

        /* Winning logic:
         * 
            -	1st "spin", user loses
            -	2nd "spin", user wins
            -	3rd-6th "spin", user loses
            -	7th, 8th "spin", user wins
            -	9th time user loses
            -	After the 10th "spin", user has a 5% chance of winning until 15 times then reset the counter

         */

        switch (timesSpinning)
        {
            case 1:
                isWin = false;
                break;
            case 2:
                isWin = true;
                break;
            case var n when (n > 2 && n < 7):
                isWin = false;
                break;
            case var n when (n == 7 || n == 8):
                isWin = true;
                break;
            case 9:
                isWin = false;
                break;
            default:
                isWin = UnityEngine.Random.value <= .05f;
                break;
        }
        Debug.Log("is win " + isWin);
        if (!isWin)
        {
            // If there is no win, let it spin to a random value but not the one the player's bet
            if (timesSpinning > 15)
            {
                timesSpinning = 0;
            }
        }
    }
    int GetDifferentIndex(int fromIndex)
    {
        List<int> newValues = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            if (i != fromIndex)
            {
                newValues.Add(i);
            }
        }
        return newValues[UnityEngine.Random.Range(0, newValues.Count)];
    }
}
