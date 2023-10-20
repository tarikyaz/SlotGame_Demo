using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;
public class SlotMachine : MonoBehaviour
{
    [SerializeField] GameObject testButtons, spining_UI, bet_UI, addCoins_UI, result_UI;
    [SerializeField] TMP_Text coinsAmount_Text, resultMsg_Text;
    [SerializeField] Button Spin_Button;
    [SerializeField] TMP_InputField betAmount_Input, amountToAdd_Input;
    [SerializeField] int[] indexesValueArray = new int[9];
    Reel[] reelsArray;
    bool isRoling => reelsArray.Any(x => x.isRoling);
    Coroutine waitForFinish;
    int betAmount => int.Parse(betAmount_Input.text);
    int amountToAdd => int.Parse(amountToAdd_Input.text);
    int lastIndex = -1;
    [Serializable]
    struct ReelData
    {
        public int reelIndex, reelValue;
    }
    ReelData[] reelResultsArray;
    private void Start()
    {
        testButtons.SetActive(false);
#if !UNITY_EDITOR
        Destroy(testButtons);
#endif
        reelsArray = GetComponentsInChildren<Reel>();
        coinsAmount_Text.text = $"{CurrentCoinsAmount} Coins";
        RestMachineToSetUI();
        Spin_Button.onClick.AddListener(() =>
        {
            if (betAmount <=0)
            {
                return;
            }
            bet_UI.SetActive(false);
            if (TrySpendCoin(betAmount))
            {
                Spin();
            }
            else
            {
                addCoins_UI.SetActive(true);
            }
        });
        betAmount_Input.onValueChanged.AddListener((v) => {
            if (string.IsNullOrEmpty(betAmount_Input.text) || betAmount <= 0)
            {
                betAmount_Input.text = "1";
            }
        });

        amountToAdd_Input.onValueChanged.AddListener((v) => {
            if (string.IsNullOrEmpty(betAmount_Input.text) || amountToAdd <= 0)
            {
                amountToAdd_Input.text = "1";
            }
        });
    }
    public void AddCoinsCall()
    {
        AddCoins(amountToAdd);
    }
    public void RestMachineToSetUI()
    {
        amountToAdd_Input.text = "1";
        betAmount_Input.text = "1";
        result_UI.SetActive(false);
        spining_UI.SetActive(false);
        bet_UI.SetActive(true);
        addCoins_UI.SetActive(false);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        SlotButton.OnClick += OnSlotButtonClickHandler;
#endif
        BaseEvents.OnCoinsAmountChange += OnCoinsAmountChangeHandler;
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        SlotButton.OnClick -= OnSlotButtonClickHandler;
#endif
        BaseEvents.OnCoinsAmountChange += OnCoinsAmountChangeHandler;

    }

    private void OnCoinsAmountChangeHandler(int value)
    {
        coinsAmount_Text.text = $"{value} Coins";
    }
    void Spin()
    {

            OnSlotButtonClickHandler(GetDifferentIndex(lastIndex));

    }

    private void OnSlotButtonClickHandler(int index)
    {
        if (!isRoling)
        {

            lastIndex = index;
            Debug.Log("Index " + index);
            spining_UI.SetActive(true);
            reelResultsArray = new ReelData[reelsArray.Length];
            if (waitForFinish != null)
            {
                StopCoroutine(waitForFinish);
            }
            BaseEvents.CallSoundPlay(SoundEffectsEnum.Click);
            TimesSpinning++;
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
                int wonAmount;
                string calculateStr = "";
                if (isWin)
                {
                    calculateStr += $"{betAmount} X {3} X {indexesValueArray[index]}\n";
                    wonAmount = betAmount * 3 * indexesValueArray[index];
                    BaseEvents.CallSoundPlay(SoundEffectsEnum.Win3);
                }
                else
                {
                    if (hasDuplicates)
                    {
                        BaseEvents.CallSoundPlay(SoundEffectsEnum.Win2);
                        calculateStr += $"{betAmount} X {2} X {indexesValueArray[index]}\n";

                        wonAmount = betAmount * 2 * indexesValueArray[index];
                    }
                    else
                    {
                        BaseEvents.CallSoundPlay(SoundEffectsEnum.Lose);

                        wonAmount = 0;
                    }
                }
                resultMsg_Text.text = calculateStr + $"You won {wonAmount} coins !";
                result_UI.SetActive(true);
                AddCoins(wonAmount);
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

        switch (TimesSpinning)
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
            if (TimesSpinning > 15)
            {
                TimesSpinning = 0;
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
