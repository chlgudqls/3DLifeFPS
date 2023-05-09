using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveNLoad theSaveNLoad;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.isPause)
                CallMenu();
            else
                CloseMenu();
        }
    }
    private void CallMenu()
    {
        GameManager.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f;
    }
    private void CloseMenu()
    {
        GameManager.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ClickSave()
    {
        // 세이브클릭하면 적용 
        Debug.Log("세이브 버튼 클릭");
        theSaveNLoad.SaveData();
    }
    public void ClickLoad()
    {
        Debug.Log("로드 버튼 클릭");
        theSaveNLoad.LoadData();
    }
    public void ClickExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
