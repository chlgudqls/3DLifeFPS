using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string closeWeaponName; //���� ���� �̸�   

    //���� ����
    public bool isHand;
    public bool isAxe;
    public bool isPickaxe;

    public float range; //���� ����
    public int damage; //���ݷ�
    public float workSpeed; //�۾� �ӵ�
    public float attackDelay; //���� ������    //�����ϰ� �� �ڿ� ���� ��õ��Ҽ��ִ� ������
    public float attackDelayA; //���� Ȱ��ȭ ����
    public float attackDelayB; //���� ��Ȱ��ȭ ����

    public float workDelay; //�۾� ������    //�����ϰ� �� �ڿ� ���� ��õ��Ҽ��ִ� ������
    public float workDelayA; //�۾� Ȱ��ȭ ����
    public float workDelayB; //�۾� ��Ȱ��ȭ ����

    public Animator anim;

}
