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
    float itemSize = (float)(1f / 12f);
    int itemsCount = 9;
    float currentYPos => material.GetTextureOffset("_MainTex").y;
    float lastItemPos => 0.75f;
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
        int pushes = UnityEngine.Random.Range(1, 4);
        rolingTween.Pause();
        rolingTween.Kill();
        if (index > itemsCount - 1 || index < 0)
        {
            Debug.LogError("no possible index " + index);
            return;
        }
        valueY = material.GetTextureOffset("_MainTex").y;
        float targetY = index * itemSize;
        targetY += pushes * itemsCount * itemSize;
        float prevYPos = currentYPos;
        bool isDing = false;
        rolingTween = DOTween.To(() => valueY, x => valueY = x, targetY, 1.5f)
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
