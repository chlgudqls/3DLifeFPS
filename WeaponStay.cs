using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStay : MonoBehaviour
{
    //������ġ
    private Vector3 originPos;

    //������ġ
    private Vector3 currentPos;
    //sway �Ѱ�
    [SerializeField]
    private Vector3 limitPos;

    //������ sway �Ѱ�
    [SerializeField]
    private Vector3 fineSightLimitPos;

    //�ε巯�� ������ ����
    [SerializeField]
    private Vector3 smoothSway;

    //�÷��� �����ͼ� �������� ���� �����Ѱ� �����ϰ��Ϸ���
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
        //0�ε� �� ���������� �����̴� �ȿ����϶� ���������� ��ġ��Ű�°ǰ� ������ ��� ����
        else
            BackToOriginPos();
    }
    private void Swaying()
    {
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if(!theGuncontroller.isFineSightMode)
        {
            //set�� ���� currentPos�� ��� ���ϴ� �� �����Ѵٴ� �Ű���
            // _moveX �� -�� ���̴��� �����غ��� ����� �����ߵɵ�
            //���콺�� ȭ���� �Ѿ ���� ���ϴϱ� ���δµ� lerp�� õõ�� �̵��ϰ��Ϸ���
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x)
                , Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y)
                //originPos z ���� �ֳ� ó�� �����Ȱ� ���� ������ �ٲ���߾�
                , originPos.z);
            //������ ���� �־���ߵ�
        }
        else
        {
            //���δ� ���� �مf�� ��������
            //smoothSway �� x,y �� �ϳ��� ��
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
