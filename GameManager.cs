using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; // 플레이어의 움직임 제어

    public static bool isOpenInventory = false; // 인벤토리 활성화 
    public static bool isOpenCraftManual = false; // 건축 메뉴창 활성화
    public static bool isOpenComputerKit = false; // 컴퓨터 키트창 활성화
    public static bool isOpenArchemyTable = false; // 연금 테이블 창 활성화

    public static bool isNight = false;
    public static bool isWater = false;

    public static bool isPause = false; // 메뉴가 호출되면 true

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
        // visible 은 포함되있음
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        theWM = FindObjectOfType<WeaponManager>();
    }

}
