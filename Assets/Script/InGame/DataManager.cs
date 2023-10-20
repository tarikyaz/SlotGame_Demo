using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager 
{
    public static int TimesSpinning
    {
        get => PlayerPrefs.GetInt("TIME_SPININNG_KEY", 0);
        set => PlayerPrefs.SetInt("TIME_SPININNG_KEY", value);
    }

    public static int CurrentCoinsAmount
    {
        get => PlayerPrefs.GetInt("CURRENT_COINS_AMOUNT_KEY", 5);
        private set
        {
            BaseEvents.CallCoinsAmountChange(value);
            PlayerPrefs.SetInt("CURRENT_COINS_AMOUNT_KEY", value);
        }
    }

    public static bool TrySpendCoin(int amount)
    {
        int newCurrentCoinsAmout = CurrentCoinsAmount - amount;
        if (newCurrentCoinsAmout<0)
        {
            return false;
        }
        CurrentCoinsAmount = newCurrentCoinsAmout;
        return true;
    }
    public static void AddCoins(int amountToAdd)
    {
        CurrentCoinsAmount += amountToAdd;
    }
}
