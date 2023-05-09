using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    //�ڽ��� �巡�׷� ��𼭵� �̰� �����Ѵٰ���
    //�ڿ� ����
    static public DragSlot instance;

    //�巡���� ������ ��ũ��Ʈ�� �����°ǵ� ���� �����Ϸ����ϴµ�
    //�ش� ��ũ��Ʈ�� ���������� ���� ȿ�����ֳ�
    public Slot dragSlot;

    //������ �̹���   
    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }
    public void DragSetImage(Image _itemImag)
    {
        // ������ �巡�׵� �������� �̹����� �ν��Ͻ��� ����
        imageItem.sprite = _itemImag.sprite;
        SetColor(1);
    }

    // ���ܵ״� ��� �̹����� ���İ��� �巡���Ҷ� �ø�
    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
