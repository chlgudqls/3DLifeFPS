using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // ȹ���� ������
    public int itemCount; // ȹ���� �������� ����
    public Image itemImage; // �������� �̹���

    [SerializeField] private bool isQuickSlot; // ������ �����Ǵ�  
    [SerializeField] private int quickNumber; // ������ ��ȣ

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    //������ �Ϸ��� �����Ŵ����� �ʿ��ϴٰ���
    // ItemEffectDatabase�� ��ġ�� �κ������� ������ ���������� ���ɿ� ���ϰɸ��⶧���� �����ű�
    //private WeaponManager theWeaponManager;
    //private Rect baseRect;
    private InputNumber theInputNumber;
    private ItemEffectDatabase theItemEffectDatabase;

    [SerializeField]
    private RectTransform baseRect;
    [SerializeField]
    private RectTransform quickSlotBaseRect; // �������� ���� 

    void Start()
    {
        // �θ��� �θ��� ��ƮƮ�������� �����ͼ� ���⶧���� 
        // ���� �ŰܾߵǴµ� �������� �θ��� �θ�� �ٸ����̱⶧���� �ƹ�ư ����̴ٸ���
        //baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        //theWeaponManager = FindObjectOfType<WeaponManager>();
        theInputNumber = FindObjectOfType<InputNumber>();
        // �������� �̰ɷ� �ٰ��� ������ ����
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }

    //�̹��� ���� ���� �κ��丮�� �����۵����� ������ �̹��� ���İ� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    // EŰ�� ������ ������ ȹ���ϸ� �������� �κ��丮�� �߰�   
    // �Ѿ���� ������Ÿ���� �޾Ƽ� ������ �̹����� ������ �̹����߰�
    // �Ѿ�� ������ ���ڵ� �����̿� �߰�
    public void AddItem(Item _item , int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = _item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            //�� �ٲ���
            //������ Ʋ���� ������ ��
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    // �Ѿ���� ���� ���� ������ �ؽ�Ʈ������ ����
     public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        //������ ��� ���Ǹ� ������ ������Ŵ
        if (itemCount <= 0)
            //������ �ٽú�� �Ѿ���� ������ �� �������
            ClearSlot();
    }
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        go_CountImage.SetActive(false);
        text_Count.text = "0";
    }

    // �̽�ũ��Ʈ�� ��ü�� ���콺�� input�ϸ� �̺�Ʈ����
    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 �̺�Ʈ�� ���� ��Ŭ���϶� �����ϰڴ�
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                //if(item.itemType == Item.ItemType.Equipment)
                //{
                    //����
                    //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                //}
                //else
                //{
                    //�Ҹ�
                    theItemEffectDatabase.UseItem(item);
                if (item.itemType == Item.ItemType.Used)
                    SetSlotCount(-1);
                //}
            }
        }
    }

    //���콺�� ��Ŭ���ؼ� ó���巡���Ҷ� �߻��ϴ� �̺�Ʈ
    //�Ű� ������ �Ѿ���°� ���콺Ŭ����ǥ�� Ŭ���� ���Կ� �Լ��� ���´�
    //�������� �����ڿ��� �� ������ �ְ� �̹������İ��ø��� ���콺�������� ������ ���콺�� ����
    //�߻��Ѵٴ°� ������ ���õƴٴ°� ���õȰ� ���Լ��� �����Ѵ�
    //� �������� �������ʿ���� Ŭ���Ѱ� �ش罽��
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null && Inventory.inventoryActivated)
        {
            //�巡���� ������ ������ �巡�׽��Կ� drag Ÿ�� ������ ���� ������������
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            //���� ������ġ�� ���ڸ��� �ΰ� �巡�׽����� �ű�
            DragSlot.instance.transform.position = eventData.position;
        }
    }
    //�巡�����ΰ�
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    //����׷� ��� ȣ��Ǵ��� OnEndDrag , OnDrop ���� �˾ƺ����
    //�巡�װ� ��������
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(baseRect.rect.xMin);
        Debug.Log(baseRect.rect.xMax);
        Debug.Log(baseRect.rect.yMax);
        Debug.Log(baseRect.rect.yMin);
        //Debug.Log(baseRect.width);
        //Debug.Log(baseRect.height);
        Debug.Log("OnEndDrag ȣ���");
        //�� �ٱ�����
        //�κ��丮�ٱ� ������ �ּڰ�,�ִ��� �̿��� 
        //if(DragSlot.instance.transform.localPosition.x < baseRect.xMin || DragSlot.instance.transform.localPosition.x > baseRect.xMax
        //    || DragSlot.instance.transform.localPosition.y < baseRect.yMin || DragSlot.instance.transform.localPosition.y > baseRect.yMax)
            //�κ��丮 ������ �ٲ�
        if (!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
            ||
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > (quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMax
            && DragSlot.instance.transform.localPosition.y < (quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMin)))
        {
            //���� ������ �ȵȱ���� ����� ó�� 
            Debug.Log("�κ��丮 ������ �����");
            if(DragSlot.instance.dragSlot != null)
            {
                //Debug.Log(quickSlotBaseRect.rect.xMin);
                //Debug.Log(quickSlotBaseRect.rect.xMax);
                //Debug.Log(quickSlotBaseRect.transform.localPosition.y);
                //Debug.Log(quickSlotBaseRect.rect.yMin);
                //Debug.Log(quickSlotBaseRect.rect.yMax);
                // �������� ��ġ������ ����Ѱ��� heigh�� 100 �̴ϱ� ����ϸ� ����
                Debug.Log((quickSlotBaseRect.transform.localPosition.y -500) - quickSlotBaseRect.rect.yMin); // -450
                Debug.Log((quickSlotBaseRect.transform.localPosition.y - 500) - quickSlotBaseRect.rect.yMax); // -550
                theInputNumber.Call();
            }
            //Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, theWeaponManager.transform.position + theWeaponManager.transform.forward, Quaternion.identity);
            //DragSlot.instance.dragSlot.ClearSlot();
        }
        else
        {
            //transform.position = originPos;
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
    }

    //����Ʈ����������
    //����������� �ش��ϴ½����� �����ϰԵ� �������� �������� ���� 
    //������ ������ �ڱ� ���Կ� �������� ���ϰ� ���� ���Կ� ���������� �ְ�
    //��ġ�������ƴ� ��������� ���Գ��� ���� �ٲ� �� ���Խ�ũ��Ʈ�� �̿��ؼ�
    //��ũ��Ʈ�ϳ��� �����ϳ� �پ��ִٴ°�
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop ȣ���");
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            // ��� �ٲٰڴܰ���
            if(isQuickSlot)  // �κ��丮���� ������ or �����Կ��� ������
            {
                //  Ȱ��ȭ�� ���������� �Ǵ� true�϶� ��ü
                // ��������� ���ѹ��� �Ѱ��ش�  �׷��� ����Ȱ��� ���ѹ��� �Ѿ�� ���Ҷ� false���Ǽ� ��ü��������ʴ´�
                theItemEffectDatabase.IsActivateQuickSlot(quickNumber);
            }
            else // �κ��丮���� �κ��丮 or �����Կ��� �κ��丮
            {
                // �����Կ��� �κ��丮 �̵�
                if (DragSlot.instance.dragSlot.isQuickSlot)
                    // Ȱ��ȭ�� ���������� �Ǵ� true�϶� ��ü
                    theItemEffectDatabase.IsActivateQuickSlot(DragSlot.instance.dragSlot.quickNumber);
            }
        }
    }
    //�̰��� ���Ե��߿� ����Ʈ���� ������ ������ ���Լ��� �����ϰ��� �̺�Ʈ��
    //�����ڿ� �̿��ؼ� ����Ҷ� 
    private void ChangeSlot()
    {
        // �������ִ��� �Ѱܳ�
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        // ħ���ϰ� ����
        // �ű�� ������ ������ ������ ��ġ�� �߰�
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            //�������� dragSlot ��ġ�� ������ ��ȯ
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        //�ٵ� �����Ѱ��� �ƹ��������ٸ� ������ �ٲ��ʿ䵵����
        else
        {
            //�Ű����� ������ġ �ʱ�ȭ
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    // ���콺�� ���Կ� ���� �ߵ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            theItemEffectDatabase.ShowToolTip(item , transform.position);
    }

    // ���콺�� ���Կ��� ���������� �ߵ�
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
    // ���ѹ��� private �ϱ� public �Լ��� �Ѱ��ذŰ� �װ� drag���� ��ߵǴϱ� 
    public int QuickSlotNumber()
    {
        return quickNumber;
    }
}
