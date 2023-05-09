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
                    //find�� �����ʰ� �ϴ� �� ����� ��ġ�� �ѱ�   
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
                else if (hitInfo.transform.tag == "WeakAnimal")
                {
                    SoundManager.instance.PlaySE("Animal_Hit");
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(1, this.transform.position);
                }

                //false���� ������ true�� ��� hit�Ǵ°� 1�� hit�ǰ� ������
                isSwing = !isSwing;
                //�浹����
                //�浹�ϸ� �������� �������� ������������ ���⼭ ����
                //�׷��� �̰� ��ӹ��������� ������Ŵ
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
