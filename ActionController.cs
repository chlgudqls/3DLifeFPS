using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능한 최대 거리.

    private bool pickUpActivated = false; // 아이템 습득 가능할 시 true
    private bool dissolveActivated = false; // 고기 해체 가능할 시 true
    private bool isDissolving = false; // 고기 해체 중에는 true
    private bool fireLookActivated = false; // 불을 근접해서 바라볼 시 true
    private bool lookComputer = false; // 컴퓨터를 바라볼 시 true  
    private bool lookArchemyTable = false; // 연금 테이블을 바라볼 시
    private bool lookActivatedTrap = false; // 가동된 함정을 바라볼 시 true

    private RaycastHit hitInfo; // 충돌체 정보 저장
     
    // 레이어 이름으로 구분
    [SerializeField]
    private LayerMask layerMast;

    // 필요한 컴포너트
    [SerializeField]
    private Text actionTxt;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private Transform tf_MeatDissolveTool; // 고기 해체 툴
    [SerializeField]
    private ComputerKit theComputer;

    [SerializeField] private string sound_meat;

    void Update()
    {
        // e 키를 누르기전부터 아이템 체크
        CheckAction();
        TryAction();
    }

    private void TryAction()
    {
        // e 키를 누르면 아이템 체크
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckAction();
            CanPickUp();
            CanMeat();
            CanDropFire();
            CanComputerPowerOn();
            CanArchemyTableOpen();
            CanReInstallTrap();
        }
    }
    //획득
    private void CanPickUp()
    {
        if (pickUpActivated)
        {
            // 정보가 있으면 
            if(hitInfo.transform != null)
            {
                //디버그자리에 인벤토리에 아이템 추가가 들어갈거임
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득했습니다");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisAppear();
            }
        }
    }
    private void CanMeat()
    {
        // 행위 가능여부
        if(dissolveActivated)
        {
            // 행위 작업
            // 근데 왜 태그를 또 확인하지 머리가 안돌아가네
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal") 
                && hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisAppear();
                // 해체 작업
                StartCoroutine(MeatCoroutine());
            }
        }
    }
    private void CanDropFire()
    {
        if(fireLookActivated)
        {
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                // 손에 들고있는 아이템을 불에 넣음 == 선택된 퀵슬롯의 아이템   (null) 일경우도있음
                Slot _selectSlot = theQuickSlot.GetSelectedSlot();
                if(_selectSlot.item != null)
                    DropAnItem(_selectSlot.item);
            } 
        }
    }
    private void CanComputerPowerOn()
    {
        if (lookComputer)
        {
            if(hitInfo.transform != null)
            {
                if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
                {

                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisAppear();
                }
            }
        }
    }
    private void CanArchemyTableOpen()
    {
        if (lookArchemyTable)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<ArchemyTable>().Window();
                InfoDisAppear();
            }
        }
    }
    private void CanReInstallTrap()
    {
        if (lookActivatedTrap)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<DeadTrap>().ReInstall();
                InfoDisAppear();
            }
        }
    }
    // 아이템의 요소를체크 name, type 사용
    private void DropAnItem(Item _selectItem)
    {
        switch (_selectItem.itemType)
        {
            case Item.ItemType.Used:
                // contains 문자열이 있는지확인 true false반환
                // 그래서 숯은 더 이상 구울수없음 생성안되서
                if(_selectItem.itemName.Contains("고기"))
                {
                    //불위에 생성시킴 
                    Instantiate(_selectItem.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                } 
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }


    IEnumerator MeatCoroutine()
    {
        // 해체중에 무기교체되면 안됨
        WeaponManager.isChangeWeapon = true;
        WeaponStay.isActivated = false;
        //미트를 실행하면 해체하는거임
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");

        PlayerController.isActivated = false;
        
        yield return new WaitForSeconds(0.2f);

        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySE(sound_meat);
        //2초 동안 해체후에 다시 사라지게함
        yield return new WaitForSeconds(1.8f);

        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().itemNumber);

        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        WeaponStay.isActivated = true;
        PlayerController.isActivated = true;
        WeaponManager.isChangeWeapon = false;
        isDissolving = false;
    }
    private void CheckAction()
    {
        // 월드의 좌표를 플레이어의 좌표(로컬)로 변환시킴
        // 레이어마스크
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMast))
        {
            if (hitInfo.transform.tag == "Item")
                // 광선을 쏴서 담긴 정보의 태그를 체크해서 아이템정보를 표시
                ItemInfoAppear();
            else if (hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")
                MeatInfoAppear();
            else if (hitInfo.transform.tag == "Fire")
                FireInfoAppear();
            else if (hitInfo.transform.tag == "Computer")
                ComputerInfoAppear();
            else if (hitInfo.transform.tag == "ArchemyTable")
                ArchemyInfoAppear();
            else if (hitInfo.transform.tag == "Trap")
                TrapInfoAppear();
            else
                InfoDisAppear();
        }
        else
            //false 이면 충돌한게 없으면 없는데 왜 활성화 됏던걸 사라지게 하기위해서
            InfoDisAppear();
    }
    private void ItemInfoAppear()
    {
        ActivatedReset();
        pickUpActivated = true;
        actionTxt.gameObject.SetActive(true);
        // 텍스트 컬러를 바꿀수있음
        actionTxt.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void MeatInfoAppear()
    {
        ActivatedReset();
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            dissolveActivated = true;
            actionTxt.gameObject.SetActive(true);
            // 텍스트 컬러를 바꿀수있음
            actionTxt.text = hitInfo.transform.GetComponent<Animal>().animalName + " 해체하기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void FireInfoAppear()
    {
        ActivatedReset();
        // 불 지속시간이있는데 불이꺼진상태일때 고기넣어도 의미가없음
        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            fireLookActivated = true;
            actionTxt.gameObject.SetActive(true);
            // 텍스트 컬러를 바꿀수있음
            // 선택된아이템을 어떻게 알고 계산하지 
            actionTxt.text = "선택된 아이템 불에 넣기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void ComputerInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            ActivatedReset();
            lookComputer = true;
            actionTxt.gameObject.SetActive(true);
            // 텍스트 컬러를 바꿀수있음
            actionTxt.text = "컴퓨터 가동 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            ActivatedReset();
            lookArchemyTable = true;
            actionTxt.gameObject.SetActive(true);
            actionTxt.text = "연금 테이블 조작 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void TrapInfoAppear()
    {
        // 이미 함정 발생해서 true 됐을때 재기동 할거라서
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            ActivatedReset();
            lookActivatedTrap = true;
            actionTxt.gameObject.SetActive(true);
            actionTxt.text = "함정 재설치 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    //획득하고난후, 다시 range범위에서 사라졌을때
    private void InfoDisAppear()
    {
        pickUpActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
        actionTxt.gameObject.SetActive(false);
    }
    // 그냥 InfoDisAppear 호출하면 텍스트가 깜빡거린다고함
    private void ActivatedReset()
    {
        pickUpActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
    }
}
