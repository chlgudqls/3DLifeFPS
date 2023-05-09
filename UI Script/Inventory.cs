using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // �κ��丮�� Ȱ��ȭ �ƴ��� ����
    public static bool inventoryActivated = false;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private GameObject go_QuickSlotsParent;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private ItemEffectDatabase theItemEffectDatabase;

    private Slot[] slots; // �κ��丮 ���Ե�
    private Slot[] quickslots; // �� ���Ե�
    private bool isNotPut;
    private int slotNumber;

    // �κ��丮�� ���Ե��� ��� ��ȯ��Ű�� ����ų����
    // �����ö� put ����
    public Slot[] GetSlots() { return slots; }

    [SerializeField] private Item[] _items;

    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < _items.Length; i++)
            if (_items[i].itemName == _itemName)
                slots[_arrayNum].AddItem(_items[i], _itemNum);
    }

    void Start() 
    {
        // ���Ե��� ������Ʈ�� �����ͼ� ������
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickslots = go_QuickSlotsParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();    
    }
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }
    private void OpenInventory()
    {
        GameManager.isOpenInventory = true;
        go_InventoryBase.SetActive(true);
    }
    private void CloseInventory()
    {
        GameManager.isOpenInventory = false;
        go_InventoryBase.SetActive(false);
        theItemEffectDatabase.HideToolTip();

    }

    //�������� �ֳѰ��� ���
    //������ ������ŭ�ΰ� 
    //�κ��丮���� ����Ѱǰ�
    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickslots, _item, _count);
        if (!isNotPut)
            theQuickSlot.IsActivateQuickSlot(slotNumber);

        if (isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("�����԰� �κ��丮�� ��á���ϴ�");
    }
    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        //���� ������ �Ʒ��� ���ϱ� �Ʒ��� ������ �󽽷Կ� ����
        //����ΰ��� �Ÿ� �ɷ����� ���� �Ʒ� for������ ���ư���������
        //�κ��丮���Կ� ȹ���� �������̸��� ��ġ�ϴ� �������̾����� �Ʒ� ���๮���� �̵�
        //�Ʒ����� ���ư��� �������� ���ο�������̰ų� ��������
        if (Item.ItemType.Equipment != _item.itemType && Item.ItemType.Kit != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    // �ѹ� �ɸ��� �����ε� �Ѿ�°� �ΰ��̻��ϸ��¾���
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }
        //�ɸ��� ������ �ٽ� ���� ""�ΰ� �ִ���
        // �ֱ��̵����� �����־�����
        for (int i = 0; i < _slots.Length; i++)
        {
            //if (slots[i].item.itemName == "")
            //���̸� itemName ���� ���������� 
            if (_slots[i].item == null)
            {
                slotNumber = i;
                //�Ѿ�°� "" ������ ī��Ʈ�� 0�̴ϱ� add�Լ��� ����
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }
        isNotPut = true;
    }
    // �������� ã���� ��ִ��� ��ȯ��Ų��
    public int GetItemCount(string _itemName)
    {
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickslots, _itemName);
    }
    // �ϴ� ������ ��ȯ��Ű�� �Ѱ����Ŀ� �ű⼭ ���ϴ°�
    private int SearchSlotItem(Slot[] _slots, string _itemName)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if(_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                    return _slots[i].itemCount;
            }
        }
        return 0;
    }
    // ������ �� ���� �ι�������
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if (!ItemCountAdjust(slots, _itemName, _itemCount)) // if�ȿ� bool�Լ��� true �� if���� �Լ��� ����Ǵ°���
            ItemCountAdjust(quickslots, _itemName, _itemCount);
    }
    private bool ItemCountAdjust(Slot[] _slots, string _itemName, int _itemCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            // ���� �κе� �����ϱ� null������ ��� �ݺ����̶� 
            // ������ ���� ���Ե� �ѹ��� ���⶧���� null�� ���ܽ��Ѿߵȴ�
            // null�� ���ϸ� ������ ��������
            if (_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                {
                    _slots[i].SetSlotCount(-_itemCount);
                    return true;
                }
            }
        }
        return false;
    }
}
