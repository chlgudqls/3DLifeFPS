using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStay : MonoBehaviour
{
    //기존위치
    private Vector3 originPos;

    //현재위치
    private Vector3 currentPos;
    //sway 한계
    [SerializeField]
    private Vector3 limitPos;

    //정조준 sway 한계
    [SerializeField]
    private Vector3 fineSightLimitPos;

    //부드러운 움직임 정도
    [SerializeField]
    private Vector3 smoothSway;

    //플래그 가져와서 정조준일 때만 가능한거 적용하게하려고
    [SerializeField]
    private GunController theGuncontroller;

    public static bool isActivated = true;
    void Start()
    {
        originPos = this.transform.localPosition;
    }

    void Update()
    {
        if (isActivated && GameManager.canPlayerMove)
            TrySway();
    }
    private void TrySway()
    {
        if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
            Swaying();
        //0인데 왜 원래값으로 움직이다 안움직일때 원래값으로 위치시키는건가 없으면 어떻게 될지
        else
            BackToOriginPos();
    }
    private void Swaying()
    {
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if(!theGuncontroller.isFineSightMode)
        {
            //set이 뭐지 currentPos에 계속 변하는 값 세팅한다는 거같음
            // _moveX 에 -왜 붙이는지 실행해보고 디버그 찍어봐야될듯
            //마우스가 화면을 넘어도 값이 변하니까 가두는데 lerp로 천천히 이동하게하려고
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x)
                , Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y)
                //originPos z 값이 있나 처음 설정된값 무슨 값인지 바꿔봐야암
                , originPos.z);
            //설정한 값을 넣어줘야됨
        }
        else
        {
            //가두는 값만 바꿧네 정조준은
            //smoothSway 를 x,y 중 하나로 씀
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x)
          , Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y)
          , originPos.z);
        }
            transform.localPosition = currentPos;
    }
    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
