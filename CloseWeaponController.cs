using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{
    //왜 이것만 private
    [SerializeField]
    protected CloseWeapon currentcloseWeapon;

    //공격중?
    protected bool isAttack = false;
    protected bool isSwing = false;
    //public static bool isActivate = false;

    protected RaycastHit hitInfo;
    public LayerMask layerMask;

    private PlayerController thePlayerController;

    void Start() 
    {
        thePlayerController = FindObjectOfType<PlayerController>();
    }

    protected void TryAttack()
    {
        if (!Inventory.inventoryActivated)
        {
             if (Input.GetButton("Fire1"))
            {
                if (!isAttack)
                {
                    if(CheckObject())
                    {
                        if(currentcloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                        {

                            StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition()));
                            StartCoroutine(AttackCoroutine("Chop", currentcloseWeapon.workDelayA, currentcloseWeapon.workDelayB, currentcloseWeapon.workDelay));
                            return;
                        }
                    }
                    StartCoroutine(AttackCoroutine("Attack", currentcloseWeapon.attackDelayA, currentcloseWeapon.attackDelayB, currentcloseWeapon.attackDelay));
                }
            }
        }
    }
    protected IEnumerator AttackCoroutine(string swingtype, float _delayA, float _delayB, float _delayC)
    {
        //애니메이션시작여부
        //false여서 실행했는데 true가 되면서 중복실행 막음
        isAttack = true;
        currentcloseWeapon.anim.SetTrigger(swingtype);

        yield return new WaitForSeconds(_delayA);
        //데미지활성여부
        isSwing = true;
        StartCoroutine(HitCoroutine());
        yield return new WaitForSeconds(_delayB);
        isSwing = false;
        //여기도 이해안감 딜레이 왜뺀거지
        //처음부터 뺀값을 인스펙터창에 넣어도 되지만 총딜레이가 1초라고 기준을 잡아서 빼는듯
        yield return new WaitForSeconds(_delayC - _delayA - _delayB);
        isAttack = false;
        //딜레이가 끝나면 false가 되면서 다시 마우스 클릭으로 공격할수있게됨
    }

    //swing 이 트루인 구간에서 오브젝트체크를 계속 해주고 체크는 bool값이고 광선을쏨
    protected abstract IEnumerator HitCoroutine();
    //{
    //    while (isSwing)
    //    {
    //        if (CheckObject())
    //        {
    //            //false여서 실행후 true로 계속 hit되는거 1번 hit되게 막아줌
    //            isSwing = !isSwing;
    //            //충돌했음
    //            //충돌하면 데미지를 입힐건지 나무를벨건지 여기서 정함
    //            //그래서 이걸 상속받은곳에서 구현시킴
    //            Debug.Log(hitInfo.transform.name);
    //        }
    //        yield return null;
    //    }
    //}
    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentcloseWeapon.range, layerMask))
        {
            return true;
        }
        return false;
    }
    //완성함수이지만 추가 수정이 가능한 함수 - 가상함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentcloseWeapon = _closeWeapon;
        //객체니까 겟컴포넌트로 transform가져오면되네
        WeaponManager.currentWeapon = currentcloseWeapon.GetComponent<Transform>();
        //
        WeaponManager.currentWeaponAnim = currentcloseWeapon.anim;
        //정조준중에 총에서 다른무기로가면 위치가바뀔수있다고하는데 모르겠다
        currentcloseWeapon.transform.localPosition = Vector3.zero;
        currentcloseWeapon.gameObject.SetActive(true);  
        //isActivate = true;
    }
}
