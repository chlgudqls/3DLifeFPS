using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    //�ڽ�Ŭ�������� �����ϴ°� �� �����Ѱ���
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
