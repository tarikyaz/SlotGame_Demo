using System;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SlotButton : MonoBehaviour
{
    public static Action<int> OnClick;
    Button btn;
    [SerializeField] int index;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            OnClick?.Invoke(index);
        });
    }
    private void Reset()
    {
        index = transform.GetSiblingIndex();
    }
}
