using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�.

    private bool pickUpActivated = false; // ������ ���� ������ �� true
    private bool dissolveActivated = false; // ��� ��ü ������ �� true
    private bool isDissolving = false; // ��� ��ü �߿��� true
    private bool fireLookActivated = false; // ���� �����ؼ� �ٶ� �� true
    private bool lookComputer = false; // ��ǻ�͸� �ٶ� �� true  
    private bool lookArchemyTable = false; // ���� ���̺��� �ٶ� ��
    private bool lookActivatedTrap = false; // ������ ������ �ٶ� �� true

    private RaycastHit hitInfo; // �浹ü ���� ����
     
    // ���̾� �̸����� ����
    [SerializeField]
    private LayerMask layerMast;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text actionTxt;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private Transform tf_MeatDissolveTool; // ��� ��ü ��
    [SerializeField]
    private ComputerKit theComputer;

    [SerializeField] private string sound_meat;

    void Update()
    {
        // e Ű�� ������������ ������ üũ
        CheckAction();
        TryAction();
    }

    private void TryAction()
    {
        // e Ű�� ������ ������ üũ
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
    //ȹ��
    private void CanPickUp()
    {
        if (pickUpActivated)
        {
            // ������ ������ 
            if(hitInfo.transform != null)
            {
                //������ڸ��� �κ��丮�� ������ �߰��� ������
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ���߽��ϴ�");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisAppear();
            }
        }
    }
    private void CanMeat()
    {
        // ���� ���ɿ���
        if(dissolveActivated)
        {
            // ���� �۾�
            // �ٵ� �� �±׸� �� Ȯ������ �Ӹ��� �ȵ��ư���
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal") 
                && hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisAppear();
                // ��ü �۾�
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
                // �տ� ����ִ� �������� �ҿ� ���� == ���õ� �������� ������   (null) �ϰ�쵵����
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
    // �������� ��Ҹ�üũ name, type ���
    private void DropAnItem(Item _selectItem)
    {
        switch (_selectItem.itemType)
        {
            case Item.ItemType.Used:
                // contains ���ڿ��� �ִ���Ȯ�� true false��ȯ
                // �׷��� ���� �� �̻� ��������� �����ȵǼ�
                if(_selectItem.itemName.Contains("���"))
                {
                    //������ ������Ŵ 
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
        // ��ü�߿� ���ⱳü�Ǹ� �ȵ�
        WeaponManager.isChangeWeapon = true;
        WeaponStay.isActivated = false;
        //��Ʈ�� �����ϸ� ��ü�ϴ°���
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");

        PlayerController.isActivated = false;
        
        yield return new WaitForSeconds(0.2f);

        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySE(sound_meat);
        //2�� ���� ��ü�Ŀ� �ٽ� ���������
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
        // ������ ��ǥ�� �÷��̾��� ��ǥ(����)�� ��ȯ��Ŵ
        // ���̾��ũ
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMast))
        {
            if (hitInfo.transform.tag == "Item")
                // ������ ���� ��� ������ �±׸� üũ�ؼ� ������������ ǥ��
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
            //false �̸� �浹�Ѱ� ������ ���µ� �� Ȱ��ȭ �Ѵ��� ������� �ϱ����ؼ�
            InfoDisAppear();
    }
    private void ItemInfoAppear()
    {
        ActivatedReset();
        pickUpActivated = true;
        actionTxt.gameObject.SetActive(true);
        // �ؽ�Ʈ �÷��� �ٲܼ�����
        actionTxt.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ�� " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void MeatInfoAppear()
    {
        ActivatedReset();
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            dissolveActivated = true;
            actionTxt.gameObject.SetActive(true);
            // �ؽ�Ʈ �÷��� �ٲܼ�����
            actionTxt.text = hitInfo.transform.GetComponent<Animal>().animalName + " ��ü�ϱ� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void FireInfoAppear()
    {
        ActivatedReset();
        // �� ���ӽð����ִµ� ���̲��������϶� ���־ �ǹ̰�����
        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            fireLookActivated = true;
            actionTxt.gameObject.SetActive(true);
            // �ؽ�Ʈ �÷��� �ٲܼ�����
            // ���õȾ������� ��� �˰� ������� 
            actionTxt.text = "���õ� ������ �ҿ� �ֱ� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void ComputerInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            ActivatedReset();
            lookComputer = true;
            actionTxt.gameObject.SetActive(true);
            // �ؽ�Ʈ �÷��� �ٲܼ�����
            actionTxt.text = "��ǻ�� ���� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            ActivatedReset();
            lookArchemyTable = true;
            actionTxt.gameObject.SetActive(true);
            actionTxt.text = "���� ���̺� ���� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    private void TrapInfoAppear()
    {
        // �̹� ���� �߻��ؼ� true ������ ��⵿ �ҰŶ�
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            ActivatedReset();
            lookActivatedTrap = true;
            actionTxt.gameObject.SetActive(true);
            actionTxt.text = "���� �缳ġ " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    //ȹ���ϰ���, �ٽ� range�������� ���������
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
    // �׳� InfoDisAppear ȣ���ϸ� �ؽ�Ʈ�� �����Ÿ��ٰ���
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
