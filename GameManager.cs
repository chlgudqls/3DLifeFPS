using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; // �÷��̾��� ������ ����

    public static bool isOpenInventory = false; // �κ��丮 Ȱ��ȭ 
    public static bool isOpenCraftManual = false; // ���� �޴�â Ȱ��ȭ
    public static bool isOpenComputerKit = false; // ��ǻ�� ŰƮâ Ȱ��ȭ
    public static bool isOpenArchemyTable = false; // ���� ���̺� â Ȱ��ȭ

    public static bool isNight = false;
    public static bool isWater = false;

    public static bool isPause = false; // �޴��� ȣ��Ǹ� true

    private WeaponManager theWM;
    private bool flag = false;

    void Update()
    {
        if (isOpenInventory || isOpenCraftManual || isOpenComputerKit || isOpenArchemyTable || isPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
            canPlayerMove = true;
        }
        if (isWater)
        {
            if (!flag)
            {
                StopAllCoroutines();
                StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        }
        else
        {
            flag = false; 
            theWM.WeaponOut();
        }
    }
    
    void Start()
    {
        // visible �� ���Ե�����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        theWM = FindObjectOfType<WeaponManager>();
    }

}
