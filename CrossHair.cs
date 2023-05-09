using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    //ũ�ν���� �������� ��Ȯ�� �������� ����
    //ũ�ν���� ���¿� ���� ���� ��Ȯ��
    private float gunAccuracy;

    //ũ�ν���� ��Ȱ��ȭ�� ���� �θ� ��ü
    //���¸� �޾ƿͼ� ũ�ν���� ���۽�ų����
    //���ڱ� �÷��̾���Ʈ�ѷ��� ��
    [SerializeField]
    private GameObject go_CrosshairHUD;
    [SerializeField]
    private GunController theGunController;
    public void WalkingAnimation(bool _flag)
    {
        //�̷��� �ϸ� ������ �����޴������� ���Եǰ��ְ� �ִϸ��̼� �̸��� ��,�ڵ� �Ȱ����ϱ� �� ���ٷ� �ذ��
        WeaponManager.currentWeaponAnim.SetBool("Walk", _flag);
        anim.SetBool("Walking", _flag);
    }
    public void RunningAnimation(bool _flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Run", _flag);
        anim.SetBool("Running", _flag);
    }
    public void JumpingAnimation(bool _flag)
    {
        anim.SetBool("Running", _flag);
    }
    public void CrouchingAnimation(bool _flag)
    {
        anim.SetBool("Crouching", _flag);
    }
    public void FineSightAnimation(bool _flag)
    {
        anim.SetBool("FineSight", _flag);
    }
    public void FireAnimation()
    {
        //Walking ���ϸ��̼��� �������̸� �� ���´ϱ� �װɷ� ����
        if (anim.GetBool("Walking"))
            anim.SetTrigger("Walk_Fire");
        else if (anim.GetBool("Crouching"))
            anim.SetTrigger("Crouch_Fire");
        else
            anim.SetTrigger("Idle_Fire"); 
    }
    public float GetAccuracy()
    {
        if (anim.GetBool("Walking"))
            gunAccuracy = 0.08f;
        else if (anim.GetBool("Crouching"))
            gunAccuracy = 0.02f;
        else if(theGunController.GetFineSightMode())
            gunAccuracy = 0.001f;
        else
            gunAccuracy = 0.04f;
        return gunAccuracy;
    }
}
