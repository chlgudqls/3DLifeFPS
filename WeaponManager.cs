using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 중복 교체 방지
    public static bool isChangeWeapon = false;
    //타입이 왜 tranform인지
    //현재 무기와 현재 무기의 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;
    //현재 무기 타입
    [SerializeField]
    private string currentWeaponType;

    //무기 교체딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //무기종류들 관리 gun류,너클류
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    //무기 접근 쉽게 가능하도록 만듦.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();

    //무기교체할때마다 스크립트 비활성화
    //중복방지
    //필요한 컴포넌트
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
        //시작할때 딕셔너리에 넣어두고 꺼내쓰는느낌
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
    //            //무기 교체 실행 (맨손)
    //            //직접 입력 swich에 들어갈 네임에 맞춰서 두번째 매개변수는 실제 무기이름
    //            StartCoroutine(ChangeWeaponCoroutine("HAND","맨손"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha2))
    //            //무기 교체 실행 (서브 머신건)
    //            StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha3))
    //            StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));

    //        else if (Input.GetKeyDown(KeyCode.Alpha4))
    //            StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "Pickaxe"));
    //    }
    //}
    //타입은 무기의종류 , 네임은 총의 종류
    //매개변수에 입력된건 바꾸려고하는것
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");
        //키누르면 현재 무기들어감 그동안의 딜레이
        yield return new WaitForSeconds(changeWeaponDelayTime);
        //이 밑에 웨폰 인 들어갈련지
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
                //무기교체할때 하면 안되는것들
                //총이면 중지해야될것들만
                //교체할때 조준상태면 조준해제
                //근데 장전 중이면
                //스탑올코루틴으로 둘다 중지된다는건가
                theGunController.CancelFineSight();
                theGunController.CancelReload();
            //_type 이게 대입하기전에 취소하는거니까 여기서 false
            //이함수로 들어오는건 이전무기인거임
                GunController.isActivate = false;
                break;
            case "HAND":
                //이거때문에 애니메이션오류
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
