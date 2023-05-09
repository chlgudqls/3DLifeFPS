using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    public static bool isActivate = true;
    void Start()
    {
        WeaponManager.currentWeapon = currentcloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentcloseWeapon.anim;
    }
    void Update()
    {
        if (isActivate)
            TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if(hitInfo.transform.tag == "Rock")
                    hitInfo.transform.GetComponent<Rock>().Mining();
                else if (hitInfo.transform.tag == "Twig")
                    //find로 하지않고 일단 이 곡괭이의 위치를 넘김   
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
                else if (hitInfo.transform.tag == "WeakAnimal")
                {
                    SoundManager.instance.PlaySE("Animal_Hit");
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(1, this.transform.position);
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
