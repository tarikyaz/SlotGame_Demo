using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class Reel : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] int number;
    [SerializeField] AnimationCurve animationCurve;
    float valueY;
    float itemSize = .1f;
    int itemsCount = 9;
    float currentYPos => material.GetTextureOffset("_MainTex").y;
    float roleSize => itemSize * itemsCount;
    float lastItemPos => 1 - itemSize;
    Material material;
    Tween rolingTween;
    private void Start()
    {
        material = Instantiate(img.material);
        img.material = material;

    }
    [ContextMenu("Test")]
    void RoleToIndex()
    {
        int pushes = 2;
        rolingTween.Pause();
        rolingTween.Kill();
        if (number > itemsCount || number < 1)
        {
            Debug.LogError("no possible index " + number);
            return;
        }
        valueY = material.GetTextureOffset("_MainTex").y;
        float targetY = number * itemSize;
        if (targetY < valueY)
        {
            targetY += roleSize;
        }
        targetY += roleSize * pushes;
        float prevYPos = currentYPos;
        bool isDing = false;
        rolingTween = DOTween.To(() => valueY, x => valueY = x, targetY, 15)
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

        }).SetEase(animationCurve);
    }
}
