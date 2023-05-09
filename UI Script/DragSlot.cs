using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    //자신을 드래그로 어디서든 이걸 대입한다고함
    //자원 공유
    static public DragSlot instance;

    //드래그한 슬롯의 스크립트를 가져온건데 무슨 짓을하려고하는듯
    //해당 스크립트를 갖고있으면 무슨 효력이있나
    public Slot dragSlot;

    //아이템 이미지   
    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }
    public void DragSetImage(Image _itemImag)
    {
        // 각각의 드래그된 아이템의 이미지를 인스턴스에 대입
        imageItem.sprite = _itemImag.sprite;
        SetColor(1);
    }

    // 숨겨뒀던 흰색 이미지의 알파값을 드래그할때 올림
    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
