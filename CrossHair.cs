using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    //크로스헤어 벌어지면 정확도 떨어지게 조절
    //크로스헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    //크로스헤어 비활성화를 위한 부모 객체
    //상태를 받아와서 크로스헤어 동작시킬거임
    //갑자기 플레이어컨트롤러로 감
    [SerializeField]
    private GameObject go_CrosshairHUD;
    [SerializeField]
    private GunController theGunController;
    public void WalkingAnimation(bool _flag)
    {
        //이렇게 하면 대입은 웨폰메니저에서 대입되고있고 애니메이션 이름은 건,핸드 똑같으니까 이 한줄로 해결됨
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
        //Walking 에니메이션이 실행중이면 그 상태니까 그걸로 구분
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
