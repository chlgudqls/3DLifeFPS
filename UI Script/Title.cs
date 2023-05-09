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
        // ���⿡ �θ� �̰��� ��������� ����� �����̱⶧���� ���������� �ı��ȴ�
        //theSaveNLoad = FindObjectOfType<SaveNLoad>();

        // �̱����� �߱⶧���� �ε���ص� ����ó�������ʰ� ���������� load �Ҽ��ִ�
        if (instance == null)
        {
            // ���� �̰� this�� �־���
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            // ����� ������ �̹� �Ѱ��� �����Ѵٴ¶��̷��� �����ǰ��� �����ǰ� �����Ǵ°Ű���
            Destroy(this.gameObject);
    }
    public void ClickStart()
    {
        Debug.Log("�ε�");
        SceneManager.LoadScene(sceneName);
        gameObject.SetActive(false);
    }
    public void ClickLoad()
    {
        Debug.Log("�ε�");
        // �׷��� ���ð� coroutine ����
        StartCoroutine(LoadCoroutine());
    }
    IEnumerator LoadCoroutine()
    {
        //SceneManager.LoadScene(sceneName);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // �� �ȿ� �ε�ȭ�� �������ִٰ���
        while (!operation.isDone)
        {
            yield return null;
        }

        //while ������ ������� ���������� �����ǵ����Ѵ� ���⼭�� ����ϱ⶧����
        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        theSaveNLoad.LoadData();
        // ȭ����ȯ ������ ������ �������� false 
        // �̱����̱⋚���� ������ ����
        // �ȵ����� �ε忡���� �����߱⶧�� 
        instance.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ClickExit()
    {
        Debug.Log("���� ����");
        Application.Quit();
    }
}
