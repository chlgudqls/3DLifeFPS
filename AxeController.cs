using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    //자식클래스에서 구분하는게 더 세밀한가봄
    public static bool isActivate = false;
    void Update()
    {
        if (isActivate)
            TryAttack();
    }
    //void Start()
    //{
    //    WeaponManager.currentWeapon = currentcloseWeapon.GetComponent<Transform>();
    //    WeaponManager.currentWeaponAnim = currentcloseWeapon.anim;
    //}
    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if (hitInfo.transform.tag == "Grass")
                {
                    hitInfo.transform.GetComponent<Grass>().Damage();
                }
                else if (hitInfo.transform.tag == "Tree")
                {
                    hitInfo.transform.GetComponent<TreeComponent>().Chop(hitInfo.point,transform.eulerAngles.y);
                }
                //false여서 실행후 true로 계속 hit되는거 1번 hit되게 막아줌
                isSwing = !isSwing;
                //충돌했음
                //충돌하면 데미지를 입힐건지 나무를벨건지 여기서 정함
                //그래서 이걸 상속받은곳에서 구현시킴
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
