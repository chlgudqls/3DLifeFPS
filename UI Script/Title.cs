using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Title : MonoBehaviour
{
    public string sceneName = "GameStage";

    public static Title instance;

    private SaveNLoad theSaveNLoad; 

    private void Awake()
    {
        // 여기에 두면 이것은 현재씬에서 만들어 진것이기때문에 다음씬에서 파괴된다
        //theSaveNLoad = FindObjectOfType<SaveNLoad>();

        // 싱글톤을 했기때문에 로드씬해도 삭제처리되지않고 정상적으로 load 할수있다
        if (instance == null)
        {
            // 원래 이건 this로 넣었음
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            // 여기로 빠지면 이미 한개가 존재한다는뜻이련지 기존의것이 보존되고 삭제되는거겠지
            Destroy(this.gameObject);
    }
    public void ClickStart()
    {
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
        gameObject.SetActive(false);
    }
    public void ClickLoad()
    {
        Debug.Log("로드");
        // 그래서 대기시간 coroutine 생성
        StartCoroutine(LoadCoroutine());
    }
    IEnumerator LoadCoroutine()
    {
        //SceneManager.LoadScene(sceneName);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 이 안에 로딩화면 넣을수있다고함
        while (!operation.isDone)
        {
            yield return null;
        }

        //while 문에서 벗어났으니 다음씬에서 생성되도록한다 여기서만 사용하기때문에
        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        theSaveNLoad.LoadData();
        // 화면전환 됐으니 기존의 검은씬은 false 
        // 싱글톤이기떄문에 삭제는 안함
        // 안된이유 로드에서만 적용했기때문 
        instance.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
