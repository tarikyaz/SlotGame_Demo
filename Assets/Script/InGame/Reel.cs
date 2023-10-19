using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
public class Reel : MonoBehaviour
{
    [SerializeField] Image icon, backGround;
    [SerializeField] AnimationCurve animationCurve;
    internal Action OnFinishRoling;
    float valueY;
    float itemSize = .1f;
    int itemsCount = 9;
    float currentYPos => material.GetTextureOffset("_MainTex").y;
    float roleSize => itemSize * itemsCount;
    float lastItemPos => 1 - itemSize;
    Material material;
    Tween rolingTween;
    internal bool isRoling;
    internal int index { get; private set; }
    Tween coloringTween;
    private void Start()
    {
        material = Instantiate(icon.material);
        icon.material = material;
        index = transform.GetSiblingIndex();
    }

    public void RoleToIndex(int index)
    {
        isRoling = true;
        coloringTween.Pause();
        coloringTween.Kill();
        coloringTween = backGround.DOColor(new Color(0, 0, 0, 0), .5f);
        int number = index + 1;
        int pushes = UnityEngine.Random.Range(1, 4);
        rolingTween.Pause();
        rolingTween.Kill();
        if (number > itemsCount || number < 1)
        {
            Debug.LogError("no possible index " + number);
            return;
        }
        valueY = material.GetTextureOffset("_MainTex").y;
        float targetY = number * itemSize;
        targetY += roleSize * pushes;
        float prevYPos = currentYPos;
        bool isDing = false;
        rolingTween = DOTween.To(() => valueY, x => valueY = x, targetY, .2f)
            .OnUpdate(() =>
        {
            float fixedValued = valueY;
            if (fixedValued < 0)
            {
                fixedValued = -valueY;
            }
            if (fixedValued > lastItemPos)
            {
                fixedValued = Mathf.Abs(valueY) % lastItemPos;
            }
            material.SetTextureOffset("_MainTex", new Vector2(0, fixedValued));
            float diff = Mathf.Abs(prevYPos - valueY);
            if (diff >= itemSize)
            {
                BaseEvents.CallSoundPlay(SoundEffectsEnum.Ding);
                prevYPos = valueY;
                isDing = !isDing;
            }

        }).OnComplete(() =>
        {
            OnFinishRoling?.Invoke();
            isRoling = false;
        })
            .SetEase(animationCurve)
            .SetSpeedBased();
    }
    public void SetColor(Color color)
    {
        coloringTween.Pause();
        coloringTween.Kill();
        coloringTween = backGround.DOColor(color, .5f).From(new Color(0, 0, 0, 0)).SetLoops(3);
    }
}
