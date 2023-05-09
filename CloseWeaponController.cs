using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{
    //�� �̰͸� private
    [SerializeField]
    protected CloseWeapon currentcloseWeapon;

    //������?
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
        //�ִϸ��̼ǽ��ۿ���
        //false���� �����ߴµ� true�� �Ǹ鼭 �ߺ����� ����
        isAttack = true;
        currentcloseWeapon.anim.SetTrigger(swingtype);

        yield return new WaitForSeconds(_delayA);
        //������Ȱ������
        isSwing = true;
        StartCoroutine(HitCoroutine());
        yield return new WaitForSeconds(_delayB);
        isSwing = false;
        //���⵵ ���ؾȰ� ������ �ֻ�����
        //ó������ ������ �ν�����â�� �־ ������ �ѵ����̰� 1�ʶ�� ������ ��Ƽ� ���µ�
        yield return new WaitForSeconds(_delayC - _delayA - _delayB);
        isAttack = false;
        //�����̰� ������ false�� �Ǹ鼭 �ٽ� ���콺 Ŭ������ �����Ҽ��ְԵ�
    }

    //swing �� Ʈ���� �������� ������Ʈüũ�� ��� ���ְ� üũ�� bool���̰� ��������
    protected abstract IEnumerator HitCoroutine();
    //{
    //    while (isSwing)
    //    {
    //        if (CheckObject())
    //        {
    //            //false���� ������ true�� ��� hit�Ǵ°� 1�� hit�ǰ� ������
    //            isSwing = !isSwing;
    //            //�浹����
    //            //�浹�ϸ� �������� �������� ������������ ���⼭ ����
    //            //�׷��� �̰� ��ӹ��������� ������Ŵ
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
    //�ϼ��Լ������� �߰� ������ ������ �Լ� - �����Լ�
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentcloseWeapon = _closeWeapon;
        //��ü�ϱ� ��������Ʈ�� transform��������ǳ�
        WeaponManager.currentWeapon = currentcloseWeapon.GetComponent<Transform>();
        //
        WeaponManager.currentWeaponAnim = currentcloseWeapon.anim;
        //�������߿� �ѿ��� �ٸ�����ΰ��� ��ġ���ٲ���ִٰ��ϴµ� �𸣰ڴ�
        currentcloseWeapon.transform.localPosition = Vector3.zero;
        currentcloseWeapon.gameObject.SetActive(true);  
        //isActivate = true;
    }
}
