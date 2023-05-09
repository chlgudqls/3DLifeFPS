using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private Slot[] quickSlots; // 퀵슬롯들
    [SerializeField] private Image[] img_CoolTime; // 퀵슬롯 쿨타임 이미지
    [SerializeField] private Transform tf_parent; // 퀵슬롯의 부모 객체

    [SerializeField] private Transform tf_ItemPos; // 아이템이 위치할 손 끝.
    // static 쓴 이유 Find 를 쓰지않기위해 효율성
    public static GameObject go_HandItem; // 손에 든 아이템.

    [SerializeField]
    private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    // 등장내용
    [SerializeField]
    private float appearTime;
    private float currentAppearTime;
    private bool isAppear;

    private int selectedSlot; // 선택된 퀵슬롯. (0~7) = 8개

    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField]
    private WeaponManager theWeaponManager;
    private Animator anim;

    void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        selectedSlot = 0;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        TryInputNumber();
        CoolTimeCalc();
        AppearCalc();
    }
    // try가 눌리면 실행
    private void AppearReset()
    {
        currentAppearTime = appearTime;
        isAppear = true;
        anim.SetBool("Appear", isAppear);
    }
    private void AppearCalc()
    {
        if(Inventory.inventoryActivated)
            AppearReset();

        else
        {
            if(isAppear)
            {
                currentAppearTime -= Time.deltaTime;
                if (currentAppearTime <= 0)
                {
                    // 실행할수있는 상태
                    isAppear = false;
                    anim.SetBool("Appear", isAppear);
                }
            }
        }
    }
    private void CoolTimeReset()
    {
        currentCoolTime = coolTime;
        isCoolTime = true;
    }
    private void CoolTimeCalc()
    {
        if(isCoolTime) 
        {
            currentCoolTime -= Time.deltaTime;
            for (int i = 0; i < img_CoolTime.Length; i++)
                img_CoolTime[i].fillAmount = currentCoolTime / coolTime;

            if (currentCoolTime <= 0)
                isCoolTime = false;
        }    
    }
    private void TryInputNumber()
    {
        if (!isCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeSlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                ChangeSlot(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                ChangeSlot(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                ChangeSlot(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                ChangeSlot(7);
        }
    }
    public void IsActivateQuickSlot(int _num)
    {
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }
        if (DragSlot.instance != null)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                if (DragSlot.instance.dragSlot.QuickSlotNumber() == selectedSlot)
                {
                    Execute();
                    return;
                }
            }
        }
    }
    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Execute();
    }
    private void SelectedSlot(int _num)
    {
        // 선택된 슬롯
        selectedSlot = _num;

        // 선택된 슬롯으로 이미지 이동
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }
    private void Execute()
    {
        AppearReset();
        CoolTimeReset();

        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            else if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Used || quickSlots[selectedSlot].item.itemType == Item.ItemType.Kit)
                ChangeHand(quickSlots[selectedSlot].item);
            else
                ChangeHand();
        }
        else
        {
            ChangeHand();
        }
    }

    private void ChangeHand(Item _item = null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if (_item != null)
            // 이안에 아이템을 손끝에 생성시킨다
            StartCoroutine(HandItemCorountine(_item)); 
    }
    IEnumerator HandItemCorountine(Item _item)
    {
        HandController.isActivate = false;

        // true가 될때까지 기다린다
        // true가 되는건 무기교체의 마지막과정이 이루졌다는것
        yield return new WaitUntil(() => HandController.isActivate);

        if (Item.ItemType.Kit == _item.itemType)
            HandController.currentKit = _item;

        // 굳이 생성시키나 활성화 비활성화가아니라
        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.transform.position, tf_ItemPos.transform.rotation);
        //부모 객체를 생성해서 따라다니게 한다고함 그냥 생성만 시키면 따라다니질 못하는지 주석 처리해보기
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;
        go_HandItem.GetComponent<BoxCollider>().enabled = false;
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = 6; // Weapon
        go_HandItem.transform.SetParent(tf_ItemPos); // tf_ItemPos 의 자식으로 들어감

    }
    public void DecreaseSelectedItem()
    {
        AppearReset();
        CoolTimeReset();
        quickSlots[selectedSlot].SetSlotCount(-1);


        if (quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem, 0.5f);
    }
    public bool GetIsCoolTime()
    {
        return isCoolTime;
    }
    public Slot GetSelectedSlot()
    {
        return quickSlots[selectedSlot];
    }
}
