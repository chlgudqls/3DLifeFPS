using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //���� �ߺ� ��ü ����
    public static bool isChangeWeapon = false;
    //Ÿ���� �� tranform����
    //���� ����� ���� ������ �ִϸ��̼�
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;
    //���� ���� Ÿ��
    [SerializeField]
    private string currentWeaponType;

    //���� ��ü������, ���� ��ü�� ������ ���� ����
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //���������� ���� gun��,��Ŭ��
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    //���� ���� ���� �����ϵ��� ����.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();

    //���ⱳü�Ҷ����� ��ũ��Ʈ ��Ȱ��ȭ
    //�ߺ�����
    //�ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController;
    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;
    [SerializeField]
    private PickaxeController thePickaxeController;


    void Start()
    {
        //�����Ҷ� ��ųʸ��� �־�ΰ� �������´���
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }
        for (int i = 0; i < axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }
        for (int i = 0; i < pickaxes.Length; i++)
        {
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
        }
    }

    //void Update()
    //{
    //    if(!isChangeWeapon)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1))
    //            //���� ��ü ���� (�Ǽ�)
    //            //���� �Է� swich�� �� ���ӿ� ���缭 �ι�° �Ű������� ���� �����̸�
    //            StartCoroutine(ChangeWeaponCoroutine("HAND","�Ǽ�"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha2))
    //            //���� ��ü ���� (���� �ӽŰ�)
    //            StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha3))
    //            StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha4))
    //            StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "Pickaxe"));
    //    }
    //}
    //Ÿ���� ���������� , ������ ���� ����
    //�Ű������� �ԷµȰ� �ٲٷ����ϴ°�
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");
        //Ű������ ���� ����� �׵����� ������
        yield return new WaitForSeconds(changeWeaponDelayTime);
        //�� �ؿ� ���� �� ������
        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }
    private void CancelPreWeaponAction()
    {
        switch(currentWeaponType)
        {
            case "GUN":
                //���ⱳü�Ҷ� �ϸ� �ȵǴ°͵�
                //���̸� �����ؾߵɰ͵鸸
                //��ü�Ҷ� ���ػ��¸� ��������
                //�ٵ� ���� ���̸�
                //��ž���ڷ�ƾ���� �Ѵ� �����ȴٴ°ǰ�
                theGunController.CancelFineSight();
                theGunController.CancelReload();
            //_type �̰� �����ϱ����� ����ϴ°Ŵϱ� ���⼭ false
            //���Լ��� �����°� ���������ΰ���
                GunController.isActivate = false;
                break;
            case "HAND":
                //�̰Ŷ����� �ִϸ��̼ǿ���
                HandController.isActivate = false;
                if (HandController.currentKit != null)
                    theHandController.Cancel();
                if (QuickSlotController.go_HandItem != null)
                    Destroy(QuickSlotController.go_HandItem);
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;
        }
    }
    private void WeaponChange(string _type, string _name)
    {
        if(_type == "GUN")
            theGunController.GunChange(gunDictionary[_name]);
        else if (_type == "HAND")
            theHandController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
        else if (_type == "PICKAXE")
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
    }

    public IEnumerator WeaponInCoroutine()
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        currentWeapon.gameObject.SetActive(false);
    }
    public void WeaponOut()
    {
        isChangeWeapon = false;

        currentWeapon.gameObject.SetActive(true);
    }
}
