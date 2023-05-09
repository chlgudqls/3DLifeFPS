using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 인벤토리가 활성화 됐는지 여부
    public static bool inventoryActivated = false;

    // 필요한 컴포넌트
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

    private Slot[] slots; // 인벤토리 슬롯들
    private Slot[] quickslots; // 퀵 슬롯들
    private bool isNotPut;
    private int slotNumber;

    // 인벤토리의 슬롯들을 모두 반환시키고 기억시킬거임
    // 가져올땐 put 으로
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
        // 슬롯들의 컴포넌트를 가져와서 뭐하지
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

    //아이템을 왜넘겨줌 어떻게
    //아이템 갯수만큼인가 
    //인벤토리에서 사용한건가
    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickslots, _item, _count);
        if (!isNotPut)
            theQuickSlot.IsActivateQuickSlot(slotNumber);

        if (isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("퀵슬롯과 인벤토리가 꽉찼습니다");
    }
    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        //장비는 무조건 아래에 들어가니까 아래에 무조건 빈슬롯에 넣음
        //장비인것은 거름 걸렀으니 장비는 아래 for문에서 돌아갈수도있음
        //인벤토리슬롯에 획득한 아이템이름과 일치하는 아이템이없으면 아래 실행문으로 이동
        //아래에서 돌아가는 아이템은 새로운아이템이거나 장비아이템
        if (Item.ItemType.Equipment != _item.itemType && Item.ItemType.Kit != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    // 한번 걸리면 리턴인데 넘어온게 두개이상일리는없나
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }
        //걸린게 없으면 다시 돌림 ""인게 있는지
        // 왜굳이돌려서 빈값을넣어주지
        for (int i = 0; i < _slots.Length; i++)
        {
            //if (slots[i].item.itemName == "")
            //널이면 itemName 까지 가지도못함 
            if (_slots[i].item == null)
            {
                slotNumber = i;
                //넘어온게 "" 없으면 카운트도 0이니까 add함수에 전달
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }
        isNotPut = true;
    }
    // 아이템을 찾으면 몇개있는지 반환시킨다
    public int GetItemCount(string _itemName)
    {
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickslots, _itemName);
    }
    // 일단 개수를 반환시키고 넘겨준후에 거기서 비교하는것
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
    // 같은걸 왜 굳이 두번만들지
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if (!ItemCountAdjust(slots, _itemName, _itemCount)) // if안에 bool함수가 true 면 if안의 함수가 실행되는건지
            ItemCountAdjust(quickslots, _itemName, _itemCount);
    }
    private bool ItemCountAdjust(Slot[] _slots, string _itemName, int _itemCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            // 없는 부분도 돌리니까 null에러가 뜬다 반복문이라서 
            // 아이템 없는 슬롯도 한번씩 돌기때문에 null은 제외시켜야된다
            // null을 비교하면 무조건 에러가뜸
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
