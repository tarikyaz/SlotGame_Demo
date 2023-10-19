using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    int timesSpinning
    {
        get => PlayerPrefs.GetInt("TIME_SPININNG_KEY", 0);
        set => PlayerPrefs.SetInt("TIME_SPININNG_KEY", value);
    }
    Reel[] reelsArray;
    private void Start()
    {
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
        timesSpinning++;
        Debug.Log("b_index " + index);
        GetResult( out var isWin);
        Debug.Log("a_index " + index);
        foreach (var reel in reelsArray)
        {
            if (!isWin)
            {
                index = GetDifferentIndex(index);
            }
            reel.RoleToIndex(index);
        }
    }
    private void GetResult(out bool isWin)
    {
        Debug.Log("timesSpinning " + timesSpinning);

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
